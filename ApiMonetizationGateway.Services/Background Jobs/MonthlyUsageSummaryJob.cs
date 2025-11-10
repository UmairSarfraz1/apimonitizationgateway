using ApiMonetizationGateway.Core.Entities;
using ApiMonetizationGateway.Core.IRepositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ApiMonetizationGateway.Services.Background_Jobs
{
    public class MonthlyUsageSummaryJob : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MonthlyUsageSummaryJob> _logger;
        private readonly TimeSpan _interval = TimeSpan.FromDays(1); 

        public MonthlyUsageSummaryJob(IServiceProvider serviceProvider, ILogger<MonthlyUsageSummaryJob> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(_interval, stoppingToken);

                    if (DateTime.UtcNow.Day == 1)
                    {
                        await GenerateMonthlySummaryAsync();
                    }
                }
                catch (Exception ex) 
                {
                    _logger.LogError(ex, "Error in monthly usage summary job");
                }
            }
        }

        private async Task GenerateMonthlySummaryAsync()
        {
            using var scope = _serviceProvider.CreateScope();
            var usageLogRepository = scope.ServiceProvider.GetRequiredService<IApiUsageLogRepository>();
            var customerRepository = scope.ServiceProvider.GetRequiredService<ICustomerRepository>();
            var monthlySummaryRepository = scope.ServiceProvider.GetRequiredService<IMonthlyUsageSummaryRepository>();

            var previousMonth = DateTime.UtcNow.AddMonths(-1);
            var monthYear = new DateTime(previousMonth.Year, previousMonth.Month, 1);

            var customers = await customerRepository.GetAllActiveAsync();

            foreach (var customer in customers)
            {
                try
                {
                    var monthlyUsage = await usageLogRepository.GetMonthlyUsageAsync(customer.CustomerId, previousMonth);
                    var amountBilled = customer.Tier.Price;

                    var summary = new MonthlyUsageSummary
                    {
                        CustomerId = customer.CustomerId,
                        MonthYear = monthYear,
                        TotalRequests = monthlyUsage,
                        AmountBilled = amountBilled
                    };

                    await monthlySummaryRepository.AddUsageSummary(summary);
                    _logger.LogInformation($"Generated monthly summary for customer {customer.CustomerId}: {monthlyUsage} requests, {amountBilled}"
                        );
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error generating monthly summary for customer {customer.CustomerId}");
                }
            }
        }

    }
}
