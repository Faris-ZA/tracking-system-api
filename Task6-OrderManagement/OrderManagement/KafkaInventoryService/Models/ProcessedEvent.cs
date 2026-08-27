using System.ComponentModel.DataAnnotations;

namespace KafkaInventoryService.Models
{
    public class ProcessedEvent
    {
        [Key]
        public Guid EventId { get; set; }

        public DateTime ProcessedAt { get; set; }
    }
}
