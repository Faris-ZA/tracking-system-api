using KafkaAnalyticsService.Models;
using Microsoft.EntityFrameworkCore;

namespace KafkaAnalyticsService.Data
{
    public class AnalyticsDbContext : DbContext
    {
        public AnalyticsDbContext(
            DbContextOptions<AnalyticsDbContext> options)
            : base(options)
        {
        }

        public DbSet<CustomerProjection> Customers { get; set; }

        public DbSet<OrderProjection> Orders { get; set; }

        public DbSet<ProcessedEvent> ProcessedEvents { get; set; }

        public DbSet<AnalyticsState> AnalyticsStates { get; set; }
    }
}
