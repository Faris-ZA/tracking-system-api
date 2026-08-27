using System.ComponentModel.DataAnnotations;

namespace KafkaAnalyticsService.Models
{
    public class CustomerProjection
    {
        [Key]
        public int CustomerId { get; set; }

        public int Status { get; set; }
    }
}
