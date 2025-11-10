using ApiMonetizationGateway.Core.Entities;
using ApiMonetizationGateway.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiMonetizationGateway.Core.IServices
{
    public interface IRateLimitingService
    {
        Task<RateLimitResponse> CheckRateLimitAsync(string apiKey, string endpoint);
        Task<ResponseModel> RecordUsageAsync(string apiKey, string endpoint);
        Task<List<MonthlyUsageSummary>> GetUserMonthlyUsageReport(string apiKey);
        Task<int> GetUserMonthlyQuota(string apiKey);
    }
}
