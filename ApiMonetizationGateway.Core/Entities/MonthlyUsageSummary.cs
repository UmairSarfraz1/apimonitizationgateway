using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiMonetizationGateway.Core.Entities
{
    public class MonthlyUsageSummary
    {
        public int SummaryId { get; set; }
        public int CustomerId { get; set; }
        public DateTime MonthYear { get; set; }
        public int TotalRequests { get; set; }
        public decimal AmountBilled { get; set; }

    }
}
