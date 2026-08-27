using System.Text.Json;
using Confluent.Kafka;
using KafkaInventoryService.Events;

namespace KafkaInventoryService.Kafka
{
    public class InventoryEventProducer : IInventoryEventProducer
    {
        private readonly IProducer<string, string> _producer;

        private const string TopicName = "inventory-events";

        public InventoryEventProducer(IConfiguration configuration)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"]
            };

            _producer = new ProducerBuilder<string, string>(config)
                .Build();
        }

        public async Task PublishAsync(InventoryEvent inventoryEvent)
        {
            var json = JsonSerializer.Serialize(inventoryEvent);

            var message = new Message<string, string>
            {
                Key = inventoryEvent.OrderId.ToString(),
                Value = json
            };

            await _producer.ProduceAsync(
                TopicName,
                message);
        }
    }
}
