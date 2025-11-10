using ApiMonetizationGateway.Core.Entities;
using ApiMonetizationGateway.Infrastructure.Data;
using ApiMonetizationGateway.Core.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace ApiMonetizationGateway.Infrastructure.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Customer>> GetAllActiveAsync() => await _context.Customers.Include(x=>x.Tier).Where(x => x.IsActive).ToListAsync();
        public async Task<Customer?> GetByApiKeyAsync(string apiKey) => await _context.Customers.Include(x=>x.Tier).Where(x => x.ApiKey == apiKey).FirstOrDefaultAsync();
    
    }
}
