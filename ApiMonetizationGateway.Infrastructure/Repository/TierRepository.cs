using ApiMonetizationGateway.Core.Entities;
using ApiMonetizationGateway.Core.IRepositories;
using ApiMonetizationGateway.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ApiMonetizationGateway.Infrastructure.Repository
{
    public class TierRepository : ITierRepository
    {
        private readonly AppDbContext _context;

        public TierRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Tier?> GetByIdAsync(int tierId) => await _context.Tiers.Where(x => x.TierId == tierId).FirstOrDefaultAsync();
        public async Task<List<Tier>> GetTierLists() => await _context.Tiers.ToListAsync();
        
    }
}
