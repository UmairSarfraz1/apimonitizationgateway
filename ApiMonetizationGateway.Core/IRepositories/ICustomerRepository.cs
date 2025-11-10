using ApiMonetizationGateway.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiMonetizationGateway.Core.IRepositories
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByApiKeyAsync(string apiKey);
        Task<List<Customer>> GetAllActiveAsync();
    }
}
