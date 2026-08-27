using KafkaOrderService.Data;
using KafkaOrderService.Models;
using Microsoft.EntityFrameworkCore;

namespace KafkaOrderService.Repositories
{
    public class LocalCustomerRepository : ILocalCustomerRepository
    {
        private readonly OrderDbContext _context;

        public LocalCustomerRepository(OrderDbContext context)
        {
            _context = context;
        }

        public async Task<LocalCustomer?> GetByIdAsync(int customerId)
        {
            return await _context.LocalCustomers
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);
        }
    }
}
