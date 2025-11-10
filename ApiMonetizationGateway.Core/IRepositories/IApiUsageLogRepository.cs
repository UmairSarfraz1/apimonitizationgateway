using ApiMonetizationGateway.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiMonetizationGateway.Core.IRepositories
{
    public interface IApiUsageLogRepository
    {
        Task<bool> AddAsync(ApiUsageLog usageLog);
        Task<int> GetMonthlyUsageAsync(int customerId, DateTime? month = null);
    }
}
