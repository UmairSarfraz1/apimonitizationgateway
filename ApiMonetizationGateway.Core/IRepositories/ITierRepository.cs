using ApiMonetizationGateway.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiMonetizationGateway.Core.IRepositories
{
    public interface ITierRepository
    {
        Task<List<Tier>> GetTierLists();
        Task<Tier?> GetByIdAsync(int tierId);

    }
}
