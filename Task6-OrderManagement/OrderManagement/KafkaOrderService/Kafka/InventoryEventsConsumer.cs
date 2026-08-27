using System.Text.Json;
using Confluent.Kafka;
using KafkaOrderService.Events;
using KafkaOrderService.Models;
using KafkaOrderService.Repositories;

namespace KafkaOrderService.Kafka
{
    public class InventoryEventsConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IOrderEventProducer _orderEventProducer;

        public InventoryEventsConsumer(
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory,
            IOrderEventProducer orderEventProducer)
        {
            _configuration = configuration;
            _scopeFactory = scopeFactory;
            _orderEventProducer = orderEventProducer;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers =
                    _configuration["Kafka:BootstrapServers"],

                GroupId = "kafka-order-inventory-group",

                AutoOffsetReset = AutoOffsetReset.Earliest,

                EnableAutoCommit = false
            };

            using var consumer =
                new ConsumerBuilder<string, string>(config)
                    .Build();

            consumer.Subscribe("inventory-events");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);

                    var inventoryEvent =
                        JsonSerializer.Deserialize<InventoryEvent>(
                            result.Message.Value);

                    if (inventoryEvent == null)
                        continue;

                    using var scope = _scopeFactory.CreateScope();

                    var repository =
                        scope.ServiceProvider
                            .GetRequiredService<IOrderRepository>();

                    var order =
                        await repository.GetByIdAsync(
                            inventoryEvent.OrderId);

                    if (order == null)
                    {
                        consumer.Commit(result);
                        continue;
                    }

                    if (inventoryEvent.EventType == "inventory-reserved")
                    {
                        order.Status = OrderStatus.Confirmed;
                    }
                    else if (inventoryEvent.EventType == "inventory-rejected")
                    {
                        order.Status = OrderStatus.Rejected;
                    }
                    else
                    {
                        consumer.Commit(result);
                        continue;
                    }

                    await repository.UpdateAsync(order);

                    var orderEventType =
                        order.Status == OrderStatus.Confirmed
                            ? "order-confirmed"
                            : "order-rejected";

                    await _orderEventProducer.PublishAsync(
                        new OrderEvent
                        {
                            EventId = Guid.NewGuid(),
                            EventType = orderEventType,
                            OrderId = order.OrderId,
                            CustomerId = order.CustomerId,
                            ProductId = order.ProductId,
                            Quantity = order.Quantity,
                            TotalAmount = order.TotalAmount,
                            OccurredAt = DateTime.UtcNow
                        });

                    consumer.Commit(result);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Inventory event consumer error: {ex.Message}");
                }
            }

            consumer.Close();
        }
    }
}
