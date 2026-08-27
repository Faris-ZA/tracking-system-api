using KafkaOrderService.Events;

namespace KafkaOrderService.Kafka
{
    public interface IOrderEventProducer
    {
        Task PublishAsync(OrderEvent orderEvent);
    }
}
