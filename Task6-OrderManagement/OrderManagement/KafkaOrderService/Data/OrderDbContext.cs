using KafkaOrderService.Models;
using Microsoft.EntityFrameworkCore;

namespace KafkaOrderService.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }

        public DbSet<LocalCustomer> LocalCustomers { get; set; }
    }
}
