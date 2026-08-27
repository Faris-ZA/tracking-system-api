using KafkaCustomerService.Events;

namespace KafkaCustomerService.Kafka
{
    public interface ICustomerEventProducer
    {
        Task PublishAsync(CustomerEvent customerEvent);
    }
}