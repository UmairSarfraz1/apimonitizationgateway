using ApiMonetizationGateway.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiMonetizationGateway.Core.IRepositories
{
    public interface IMonthlyUsageSummaryRepository
    {
        Task<bool> AddUsageSummary(MonthlyUsageSummary summary);
        Task<List<MonthlyUsageSummary>> GetUserMonthlyUsageReport(int customerId);
    }
}
