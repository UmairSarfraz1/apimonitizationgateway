using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiMonetizationGateway.Core.Entities
{
    public class ApiUsageLog
    {
        public long LogId { get; set; }
        public int CustomerId { get; set; }
        public Guid UserId { get; set; }
        public string Endpoint { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public Customer? Customer { get; set; } = null;
    }
}
