using ApiMonetizationGateway.Core.Entities;
using ApiMonetizationGateway.Core.IRepositories;
using ApiMonetizationGateway.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiMonetizationGateway.Infrastructure.Repository
{
    public class MonthlyUsageSummaryRepository : IMonthlyUsageSummaryRepository
    {
        private readonly AppDbContext _context;

        public MonthlyUsageSummaryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> AddUsageSummary(MonthlyUsageSummary summary)
        {
            await _context.AddAsync(summary);
            var response = await _context.SaveChangesAsync();
            return response != 0 ? true : false;
        }
          
        public async Task<List<MonthlyUsageSummary>> GetUserMonthlyUsageReport(int customerId) => await _context.MonthlyUsageSummary.Where(x=>x.CustomerId == customerId).OrderByDescending(x=>x.MonthYear).ToListAsync();
      
    }
}
