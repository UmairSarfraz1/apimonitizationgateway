using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiMonetizationGateway.Core.Models
{
    public class RateLimitResponse
    {
        public bool IsAllowed { get; init; }
        public string? ErrorMessage { get; init; }
        public int ErrorCode { get; init; }
        public int? RetryAfterSeconds { get; init; }
    }
}
