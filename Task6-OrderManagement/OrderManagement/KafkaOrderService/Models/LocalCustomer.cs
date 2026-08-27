using System.ComponentModel.DataAnnotations;

namespace KafkaOrderService.Models
{
    public class LocalCustomer
    {
        [Key]
        public int CustomerId { get; set; }

        public int Status { get; set; }
    }
}
