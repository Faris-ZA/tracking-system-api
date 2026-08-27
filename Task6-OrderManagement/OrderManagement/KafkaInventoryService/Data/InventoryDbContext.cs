using KafkaInventoryService.Models;
using Microsoft.EntityFrameworkCore;

namespace KafkaInventoryService.Data
{
    public class InventoryDbContext : DbContext
    {
        public InventoryDbContext(
            DbContextOptions<InventoryDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<ProcessedEvent> ProcessedEvents { get; set; }
    }
}
