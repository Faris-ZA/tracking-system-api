using System.Text.Json;
using Confluent.Kafka;
using KafkaOrderService.Events;

namespace KafkaOrderService.Kafka
{
    public class OrderEventProducer : IOrderEventProducer
    {
        private readonly IProducer<string, string> _producer;

        private const string TopicName = "order-events";

        public OrderEventProducer(IConfiguration configuration)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"]
            };

            _producer = new ProducerBuilder<string, string>(config)
                .Build();
        }

        public async Task PublishAsync(OrderEvent orderEvent)
        {
            var json = JsonSerializer.Serialize(orderEvent);

            var message = new Message<string, string>
            {
                Key = orderEvent.OrderId.ToString(),
                Value = json
            };

            await _producer.ProduceAsync(
                TopicName,
                message);
        }
    }
}
