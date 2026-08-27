using System.ComponentModel.DataAnnotations;

namespace KafkaAnalyticsService.Models
{
    public class OrderProjection
    {
        [Key]
        public int OrderId { get; set; }

        public int Status { get; set; }
    }
}
