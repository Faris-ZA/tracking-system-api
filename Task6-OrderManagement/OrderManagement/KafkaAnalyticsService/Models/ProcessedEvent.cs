using System.ComponentModel.DataAnnotations;

namespace KafkaAnalyticsService.Models
{
    public class ProcessedEvent
    {
        [Key]
        public Guid EventId { get; set; }

        public DateTime ProcessedAt { get; set; }
    }
}
