using ApiMonetizationGateway.Core.Entities;
using ApiMonetizationGateway.Core.IRepositories;
using ApiMonetizationGateway.Services.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;

namespace ApiMonetizationGateway.API.Tests;

[TestFixture]
public class RateLimitingServiceTest
{
    private Mock<ICustomerRepository> _mockCustomerRepo;
    private Mock<IApiUsageLogRepository> _mockApiUsageLogRepo;
    private Mock<ITierRepository> _mockTierRepo;
    private Mock<IMonthlyUsageSummaryRepository> _mockmonthlyUsageSummaryRepo;
    private IMemoryCache _memoryCache;
    private Mock<ILogger<RateLimitingService>> _mockLogger;
    private RateLimitingService _rateLimitingService;

    [SetUp]
    public void Setup()
    {
        _mockCustomerRepo = new Mock<ICustomerRepository>();
        _mockApiUsageLogRepo = new Mock<IApiUsageLogRepository>();
        _mockTierRepo = new Mock<ITierRepository>();
        _mockmonthlyUsageSummaryRepo = new Mock<IMonthlyUsageSummaryRepository>();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _mockLogger = new Mock<ILogger<RateLimitingService>>();

        _rateLimitingService = new RateLimitingService(
            _mockCustomerRepo.Object,
            _mockApiUsageLogRepo.Object,
            _mockTierRepo.Object,
            _memoryCache,
            _mockLogger.Object,
            _mockmonthlyUsageSummaryRepo.Object);

    }


    [TearDown]
    public void TearDown()
    {
        _memoryCache?.Dispose();
    }

    [Test]
    public async Task CheckRateLimit_WithinMonthlyQuota_ReturnAllowed()
    {
        //Arrange 
        var apiKey = "asftdtyfqwy2332jb423ui4b3u2b456";
        var endpoint = "/api/Test";

        var customer = new Customer() { CustomerId = 1, UserId = new Guid("5cd25fa8-b3b3-4926-96b3-aa111c9fee75"), Email = "umair@gmail.com", Name = "Umair", TierId = 1, ApiKey = "asftdtyfqwy2332jb423ui4b3u2b456", IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) };
        var tier = new Tier() { TierId = 1, Name = "Free", Price = 0m, MonthlyQuota = 100, RateLimit = 2 };

        _mockCustomerRepo.Setup(x => x.GetByApiKeyAsync(apiKey))
                         .ReturnsAsync(customer);
        _mockTierRepo.Setup(x => x.GetByIdAsync(customer.TierId))
                     .ReturnsAsync(tier);
        _mockApiUsageLogRepo.Setup(x => x.GetMonthlyUsageAsync(customer.CustomerId, null))
                            .ReturnsAsync(80);

        //Act
        var result = await _rateLimitingService.CheckRateLimitAsync(apiKey, endpoint);

        //Assert
        Assert.True(result.IsAllowed);
    }


    [Test]
    public async Task CheckRateLimit_WhenMonthlyQuotaExceeded_ReturnNotAllowed()
    {
        // Arrange 
        var apiKey = "asftdtyfqwy2332jb423ui4b3u2b456";
        var endpoint = "/api/Test";

        var customer = new Customer() { CustomerId = 1, UserId = new Guid("5cd25fa8-b3b3-4926-96b3-aa111c9fee75"), Email = "umair@gmail.com", Name = "Umair", TierId = 1, ApiKey = "asftdtyfqwy2332jb423ui4b3u2b456", IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) };
        var tier = new Tier() { TierId = 1, Name = "Free", Price = 0m, MonthlyQuota = 100, RateLimit = 2 };

        _mockCustomerRepo.Setup(x => x.GetByApiKeyAsync(apiKey))
                         .ReturnsAsync(customer);
        _mockTierRepo.Setup(x => x.GetByIdAsync(customer.TierId))
                     .ReturnsAsync(tier);
        _mockApiUsageLogRepo.Setup(x => x.GetMonthlyUsageAsync(customer.CustomerId, null))
                            .ReturnsAsync(101); 

        // Act
        var result = await _rateLimitingService.CheckRateLimitAsync(apiKey, endpoint);

        // Assert
        Assert.False(result.IsAllowed);
        Assert.That(result.ErrorCode == 429,result.ErrorMessage);
    }

    [Test]
    public async Task CheckRateLimit_RateLimitNotExceeded_ReturnAllowed()
    {
        // Arrange 
        var apiKey = "asftdtyfqwy2332jb423ui4b3u2b456";
        var endpoint = "/api/Test";

        var customer = new Customer() { CustomerId = 1, UserId = new Guid("5cd25fa8-b3b3-4926-96b3-aa111c9fee75"), Email = "umair@gmail.com", Name = "Umair", TierId = 1, ApiKey = "asftdtyfqwy2332jb423ui4b3u2b456", IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) };
        var tier = new Tier() { TierId = 1, Name = "Free", Price = 0m, MonthlyQuota = 100, RateLimit = 2 };

        _mockCustomerRepo.Setup(x => x.GetByApiKeyAsync(apiKey))
                         .ReturnsAsync(customer);
        _mockTierRepo.Setup(x => x.GetByIdAsync(customer.TierId))
                     .ReturnsAsync(tier);
        _mockApiUsageLogRepo.Setup(x => x.GetMonthlyUsageAsync(customer.CustomerId, null))
                            .ReturnsAsync(50);

        var rateLimitKey = $"1:{DateTime.UtcNow:yyyyMMddHHmmss}";
        _memoryCache.Set(rateLimitKey, 1, TimeSpan.FromSeconds(1));

        // Act
        var result = await _rateLimitingService.CheckRateLimitAsync(apiKey, endpoint);

        // Assert
        Assert.True(result.IsAllowed);
    }



    [Test]
    public async Task CheckRateLimit_WhenRateLimitExceeded_ReturnNotAllowed()
    {
        // Arrange 
        var apiKey = "asftdtyfqwy2332jb423ui4b3u2b456";
        var endpoint = "/api/Test";

        var customer = new Customer() { CustomerId = 1, UserId = new Guid("5cd25fa8-b3b3-4926-96b3-aa111c9fee75"), Email = "umair@gmail.com", Name = "Umair", TierId = 1, ApiKey = "asftdtyfqwy2332jb423ui4b3u2b456", IsActive = true, CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) };
        var tier = new Tier() { TierId = 1, Name = "Free", Price = 0m, MonthlyQuota = 100, RateLimit = 2 };

        _mockCustomerRepo.Setup(x => x.GetByApiKeyAsync(apiKey))
                         .ReturnsAsync(customer);
        _mockTierRepo.Setup(x => x.GetByIdAsync(customer.TierId))
                     .ReturnsAsync(tier);
        _mockApiUsageLogRepo.Setup(x => x.GetMonthlyUsageAsync(customer.CustomerId, null))
                            .ReturnsAsync(50);

        var rateLimitKey = $"1:{DateTime.UtcNow:yyyyMMddHHmmss}";
        _memoryCache.Set(rateLimitKey, 2, TimeSpan.FromSeconds(1));

        // Act
        var result = await _rateLimitingService.CheckRateLimitAsync(apiKey, endpoint);

        // Assert
        Assert.False(result.IsAllowed);
        Assert.That(result.ErrorCode == 429, result.ErrorMessage);
        Assert.That(result.RetryAfterSeconds == 1);
    }


    [Test]
    public async Task CheckRateLimit_WithInvalidApiKey_ReturnNotAllowed()
    {
        // Arrange 
        var apiKey = "asdfghjklqwertyuiopwefef";
        var endpoint = "/api/Test";

        _mockCustomerRepo.Setup(x => x.GetByApiKeyAsync(apiKey))
                         .ReturnsAsync((Customer)null);

        // Act
        var result = await _rateLimitingService.CheckRateLimitAsync(apiKey, endpoint);

        // Assert
        Assert.False(result.IsAllowed);
        Assert.That(result.ErrorMessage == "Invalid API key.");
    }




}
