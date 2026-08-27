using System.Text.Json;
using Confluent.Kafka;
using KafkaCustomerService.Events;

namespace KafkaCustomerService.Kafka
{
    public class CustomerEventProducer : ICustomerEventProducer
    {
        private readonly IProducer<string, string> _producer;

        private const string TopicName = "customer-events";

        public CustomerEventProducer(IConfiguration configuration)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"]
            };

            _producer = new ProducerBuilder<string, string>(config)
                .Build();
        }

        public async Task PublishAsync(CustomerEvent customerEvent)
        {
            var json = JsonSerializer.Serialize(customerEvent);

            var message = new Message<string, string>
            {
                Key = customerEvent.CustomerId.ToString(),
                Value = json
            };

            await _producer.ProduceAsync(
                TopicName,
                message);
        }
    }
}