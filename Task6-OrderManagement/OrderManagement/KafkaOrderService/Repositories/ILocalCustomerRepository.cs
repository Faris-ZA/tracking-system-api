using KafkaOrderService.Models;

namespace KafkaOrderService.Repositories
{
    public interface ILocalCustomerRepository
    {
        Task<LocalCustomer?> GetByIdAsync(int customerId);
    }
}
