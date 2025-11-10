using ApiMonetizationGateway.Core.Entities;
using ApiMonetizationGateway.Core.IRepositories;
using ApiMonetizationGateway.Core.IServices;
using ApiMonetizationGateway.Core.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ApiMonetizationGateway.Services.Services
{
    public class RateLimitingService : IRateLimitingService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IApiUsageLogRepository _usageLogRepository;
        private readonly ITierRepository _tierRepository;
        private readonly IMonthlyUsageSummaryRepository _monthlyUsageSummaryRepo;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<RateLimitingService> _logger;

        public RateLimitingService(ICustomerRepository customerRepository, IApiUsageLogRepository usageLogRepository, ITierRepository tierRepository, IMemoryCache memoryCache, ILogger<RateLimitingService> logger, IMonthlyUsageSummaryRepository monthlyUsageSummaryRepo)
        {
            _customerRepository = customerRepository;
            _usageLogRepository = usageLogRepository;
            _tierRepository = tierRepository;
            _memoryCache = memoryCache;
            _logger = logger;
            _monthlyUsageSummaryRepo = monthlyUsageSummaryRepo;
        }

        public async Task<RateLimitResponse> CheckRateLimitAsync(string apiKey, string endpoint)
        {
            RateLimitResponse response;
            try
            {
                var customer = await _customerRepository.GetByApiKeyAsync(apiKey);

                if (customer is null)
                    return response = new RateLimitResponse() { IsAllowed = false, ErrorMessage = "Invalid API key." };

                var tierInfo = await _tierRepository.GetByIdAsync(customer.TierId);
                if (tierInfo is null)
                    return response = new RateLimitResponse() { IsAllowed = false, ErrorMessage = "Tier Recognition failed." };

                //Checking the monthly quota of requests
                var monthlyUsage = await _usageLogRepository.GetMonthlyUsageAsync(customer.CustomerId);

                if (monthlyUsage >= tierInfo.MonthlyQuota)
                {
                    _logger.LogWarning($"Monthly Quota limit reached for the customer ID : {customer.CustomerId}");
                    return response = new RateLimitResponse() { IsAllowed = false, ErrorMessage = "Monthly Quota limit reached", ErrorCode = 429 };
                }

                //Checking the rate limit of request per second
                var rateLimitKey = $"{customer.CustomerId}:{DateTime.UtcNow:yyyyMMddHHmmss}";
                var currentSecondCount = _memoryCache.GetOrCreate(rateLimitKey, entry =>
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1);
                    return 0;
                });

                if (currentSecondCount >= tierInfo.RateLimit)
                {
                    _logger.LogWarning("Rate limit exceeded for customer {CustomerId}", customer.CustomerId);
                    return response = new RateLimitResponse() { IsAllowed = false, RetryAfterSeconds = 1, ErrorMessage = $"Rate limit of {tierInfo.RateLimit} requests per second exceeded", ErrorCode = 429 };
                }

                _memoryCache.Set(rateLimitKey, currentSecondCount + 1);


                return response = new RateLimitResponse() { IsAllowed = true };

            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking rate limit for API key {apiKey}, {ex.Message}");
                return response = new RateLimitResponse() { IsAllowed = false, ErrorMessage = "Monthly Quota limit reached" };
            }

        }

        public async Task<ResponseModel> RecordUsageAsync(string apiKey, string endpoint)
        {

            ResponseModel response;
            try
            {
                var customer = await _customerRepository.GetByApiKeyAsync(apiKey);
                if (customer is null)
                    return response = new ResponseModel() { IsSuccess = false, Message = "Invalid API key." };

                ApiUsageLog usageLog = new ApiUsageLog()
                {
                    UserId = customer.UserId,
                    CustomerId = customer.CustomerId,
                    Endpoint = endpoint,
                    Timestamp = DateTime.UtcNow
                };

                var result = await _usageLogRepository.AddAsync(usageLog);

                return result ? response = new ResponseModel() { IsSuccess = true, Message = "Usage Log Added." }
                              : new ResponseModel() { IsSuccess = false, Message = "Failed to add usage log." }; ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording usage for API key {ApiKey}", apiKey);
                return response = new ResponseModel() { IsSuccess = false, Message = "Internal Server Error." };
            }
        }


        public async Task<List<MonthlyUsageSummary>> GetUserMonthlyUsageReport(string apiKey)
        {
            var customer = await _customerRepository.GetByApiKeyAsync(apiKey);

            if (customer is null)
                return null;

            var usageReport = await _monthlyUsageSummaryRepo.GetUserMonthlyUsageReport(customer.CustomerId);
            return usageReport;
        }

        public async Task<int> GetUserMonthlyQuota(string apiKey)
        {
            var customer = await _customerRepository.GetByApiKeyAsync(apiKey);

            if (customer is null)
                return 0;

           return await _usageLogRepository.GetMonthlyUsageAsync(customer.CustomerId);
        }
    }
}
