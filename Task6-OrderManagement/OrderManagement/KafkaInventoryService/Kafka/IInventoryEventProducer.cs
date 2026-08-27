using KafkaInventoryService.Events;

namespace KafkaInventoryService.Kafka
{
    public interface IInventoryEventProducer
    {
        Task PublishAsync(InventoryEvent inventoryEvent);
    }
}
