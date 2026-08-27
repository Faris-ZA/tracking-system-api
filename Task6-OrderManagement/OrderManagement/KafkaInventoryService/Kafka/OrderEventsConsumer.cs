using System.Text.Json;
using Confluent.Kafka;
using KafkaInventoryService.Data;
using KafkaInventoryService.Events;
using KafkaInventoryService.Models;
using Microsoft.EntityFrameworkCore;

namespace KafkaInventoryService.Kafka
{
    public class OrderEventsConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IInventoryEventProducer _eventProducer;

        public OrderEventsConsumer(
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory,
            IInventoryEventProducer eventProducer)
        {
            _configuration = configuration;
            _scopeFactory = scopeFactory;
            _eventProducer = eventProducer;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers =
                    _configuration["Kafka:BootstrapServers"],

                GroupId = "kafka-inventory-order-group",

                AutoOffsetReset = AutoOffsetReset.Earliest,

                EnableAutoCommit = false
            };

            using var consumer =
                new ConsumerBuilder<string, string>(config)
                    .Build();

            consumer.Subscribe("order-events");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);

                    var orderEvent =
                        JsonSerializer.Deserialize<OrderEvent>(
                            result.Message.Value);

                    if (orderEvent == null)
                        continue;

                    // Inventory only handles new orders.
                    if (orderEvent.EventType != "order-created")
                    {
                        consumer.Commit(result);
                        continue;
                    }

                    using var scope = _scopeFactory.CreateScope();

                    var dbContext =
                        scope.ServiceProvider
                            .GetRequiredService<InventoryDbContext>();

                    // ------------------------------------------------
                    // IDEMPOTENCY CHECK
                    // ------------------------------------------------

                    var alreadyProcessed =
                        await dbContext.ProcessedEvents
                            .AnyAsync(
                                e => e.EventId == orderEvent.EventId,
                                stoppingToken);

                    if (alreadyProcessed)
                    {
                        Console.WriteLine(
                            $"Duplicate event ignored: {orderEvent.EventId}");

                        consumer.Commit(result);
                        continue;
                    }

                    // ------------------------------------------------
                    // Transaction:
                    // stock change + ProcessedEvent succeed together
                    // ------------------------------------------------

                    await using var transaction =
                        await dbContext.Database.BeginTransactionAsync(
                            stoppingToken);

                    var product =
                        await dbContext.Products
                            .FirstOrDefaultAsync(
                                p => p.Id == orderEvent.ProductId,
                                stoppingToken);

                    string resultEventType;

                    if (product == null ||
                        orderEvent.Quantity <= 0 ||
                        product.AvailableQuantity < orderEvent.Quantity)
                    {
                        resultEventType = "inventory-rejected";
                    }
                    else
                    {
                        product.AvailableQuantity -= orderEvent.Quantity;
                        product.UpdatedDate = DateTime.UtcNow;

                        resultEventType = "inventory-reserved";
                    }

                    dbContext.ProcessedEvents.Add(
                        new ProcessedEvent
                        {
                            EventId = orderEvent.EventId,
                            ProcessedAt = DateTime.UtcNow
                        });

                    await dbContext.SaveChangesAsync(stoppingToken);

                    await transaction.CommitAsync(stoppingToken);

                    // ------------------------------------------------
                    // Tell OrderService what Inventory decided
                    // ------------------------------------------------

                    await _eventProducer.PublishAsync(
                        new InventoryEvent
                        {
                            EventId = Guid.NewGuid(),
                            EventType = resultEventType,
                            OrderId = orderEvent.OrderId,
                            ProductId = orderEvent.ProductId,
                            Quantity = orderEvent.Quantity,
                            OccurredAt = DateTime.UtcNow
                        });

                    // Mark Kafka record successfully consumed
                    consumer.Commit(result);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Order event consumer error: {ex.Message}");
                }
            }

            consumer.Close();
        }
    }
}
