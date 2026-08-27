// placeholder
using CustomerService.Models;

namespace CustomerService.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(int id);
        Task<List<Customer>> GetAllAsync();
        Task<Customer> CreateAsync(Customer customer);
        Task UpdateAsync(Customer customer);
    }
}