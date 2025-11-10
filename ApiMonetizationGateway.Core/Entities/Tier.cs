using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiMonetizationGateway.Core.Entities
{
    public class Tier
    {
        public int TierId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int MonthlyQuota { get; set; }
        public int RateLimit { get; set; }
    }
}
