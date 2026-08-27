// placeholder
using KafkaCustomerService.Models;

namespace KafkaCustomerService.Repositories
{
    public interface ICustomerRepository
    {
        Task<Customer?> GetByIdAsync(int id);
        Task<List<Customer>> GetAllAsync();
        Task<Customer> CreateAsync(Customer customer);
        Task UpdateAsync(Customer customer);
    }
}
