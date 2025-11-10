using ApiMonetizationGateway.Core.Entities;
using ApiMonetizationGateway.Core.IRepositories;
using ApiMonetizationGateway.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiMonetizationGateway.Infrastructure.Repository
{
    public class ApiUsageLogRepository : IApiUsageLogRepository
    {
        private readonly AppDbContext _context;

        public ApiUsageLogRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddAsync(ApiUsageLog usageLog)
        {
            await _context.AddAsync(usageLog);
            var response = await _context.SaveChangesAsync();
            return response != 0 ? true : false;
        }

        public async Task<int> GetMonthlyUsageAsync(int customerId, DateTime? month = null)
        {
            var targetMonth = month ?? DateTime.UtcNow;

            var startDate = new DateTime(targetMonth.Year, targetMonth.Month, 1);
            var lastDate = startDate.AddMonths(1);

            return await _context.ApiUsageLogs
                                           .Where(x => x.CustomerId == customerId
                                           && x.Timestamp >= startDate
                                           && x.Timestamp < lastDate
                                           )
                                           .CountAsync();
        }
    }
}
