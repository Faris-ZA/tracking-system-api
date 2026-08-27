using System.Text.Json;
using Confluent.Kafka;
using KafkaAnalyticsService.Data;
using KafkaAnalyticsService.Events;
using KafkaAnalyticsService.Models;
using Microsoft.EntityFrameworkCore;

namespace KafkaAnalyticsService.Kafka
{
    public class AnalyticsEventsConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;

        public AnalyticsEventsConsumer(
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory)
        {
            _configuration = configuration;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers =
                    _configuration["Kafka:BootstrapServers"],

                GroupId = "kafka-analytics-group",

                AutoOffsetReset = AutoOffsetReset.Earliest,

                EnableAutoCommit = false
            };

            using var consumer =
                new ConsumerBuilder<string, string>(config)
                    .Build();

            consumer.Subscribe(
                new[]
                {
                    "customer-events",
                    "order-events",
                    "inventory-events"
                });

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result =
                        consumer.Consume(stoppingToken);

                    using var scope =
                        _scopeFactory.CreateScope();

                    var db =
                        scope.ServiceProvider
                            .GetRequiredService<AnalyticsDbContext>();

                    Guid eventId;

                    if (result.Topic == "customer-events")
                    {
                        var customerEvent =
                            JsonSerializer.Deserialize<CustomerEvent>(
                                result.Message.Value);

                        if (customerEvent == null)
                            continue;

                        eventId = customerEvent.EventId;

                        if (await IsProcessedAsync(
                            db,
                            eventId,
                            stoppingToken))
                        {
                            consumer.Commit(result);
                            continue;
                        }

                        await HandleCustomerEventAsync(
                            db,
                            customerEvent,
                            stoppingToken);
                    }
                    else if (result.Topic == "order-events")
                    {
                        var orderEvent =
                            JsonSerializer.Deserialize<OrderEvent>(
                                result.Message.Value);

                        if (orderEvent == null)
                            continue;

                        eventId = orderEvent.EventId;

                        if (await IsProcessedAsync(
                            db,
                            eventId,
                            stoppingToken))
                        {
                            consumer.Commit(result);
                            continue;
                        }

                        await HandleOrderEventAsync(
                            db,
                            orderEvent,
                            stoppingToken);
                    }
                    else
                    {
                        var inventoryEvent =
                            JsonSerializer.Deserialize<InventoryEvent>(
                                result.Message.Value);

                        if (inventoryEvent == null)
                            continue;

                        eventId = inventoryEvent.EventId;

                        if (await IsProcessedAsync(
                            db,
                            eventId,
                            stoppingToken))
                        {
                            consumer.Commit(result);
                            continue;
                        }

                        await HandleInventoryEventAsync(
                            db,
                            inventoryEvent,
                            stoppingToken);
                    }

                    db.ProcessedEvents.Add(
                        new ProcessedEvent
                        {
                            EventId = eventId,
                            ProcessedAt = DateTime.UtcNow
                        });

                    await db.SaveChangesAsync(stoppingToken);

                    consumer.Commit(result);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Analytics consumer error: {ex.Message}");
                }
            }

            consumer.Close();
        }


        private static async Task<bool> IsProcessedAsync(
            AnalyticsDbContext db,
            Guid eventId,
            CancellationToken cancellationToken)
        {
            return await db.ProcessedEvents
                .AnyAsync(
                    e => e.EventId == eventId,
                    cancellationToken);
        }


        private static async Task<AnalyticsState> GetStateAsync(
            AnalyticsDbContext db,
            CancellationToken cancellationToken)
        {
            var state =
                await db.AnalyticsStates
                    .FirstOrDefaultAsync(
                        s => s.Id == 1,
                        cancellationToken);

            if (state != null)
                return state;

            state = new AnalyticsState
            {
                Id = 1
            };

            db.AnalyticsStates.Add(state);

            return state;
        }


        private static async Task HandleCustomerEventAsync(
            AnalyticsDbContext db,
            CustomerEvent customerEvent,
            CancellationToken cancellationToken)
        {
            var state =
                await GetStateAsync(
                    db,
                    cancellationToken);

            var customer =
                await db.Customers
                    .FirstOrDefaultAsync(
                        c =>
                            c.CustomerId ==
                            customerEvent.CustomerId,
                        cancellationToken);

            if (customerEvent.EventType == "customer-created")
            {
                if (customer != null)
                    return;

                db.Customers.Add(
                    new CustomerProjection
                    {
                        CustomerId =
                            customerEvent.CustomerId,

                        Status =
                            customerEvent.Status
                    });

                state.TotalCustomers++;

                if (customerEvent.Status == 0)
                    state.ActiveCustomers++;
                else
                    state.InactiveCustomers++;
            }
            else if (
                customerEvent.EventType == "customer-updated")
            {
                if (customer == null)
                {
                    customer =
                        new CustomerProjection
                        {
                            CustomerId =
                                customerEvent.CustomerId,

                            Status =
                                customerEvent.Status
                        };

                    db.Customers.Add(customer);

                    state.TotalCustomers++;

                    if (customerEvent.Status == 0)
                        state.ActiveCustomers++;
                    else
                        state.InactiveCustomers++;

                    return;
                }

                if (customer.Status != customerEvent.Status)
                {
                    if (customer.Status == 0)
                        state.ActiveCustomers--;
                    else
                        state.InactiveCustomers--;

                    if (customerEvent.Status == 0)
                        state.ActiveCustomers++;
                    else
                        state.InactiveCustomers++;
                }

                customer.Status = customerEvent.Status;
            }
        }


        private static async Task HandleOrderEventAsync(
            AnalyticsDbContext db,
            OrderEvent orderEvent,
            CancellationToken cancellationToken)
        {
            var state =
                await GetStateAsync(
                    db,
                    cancellationToken);

            var order =
                await db.Orders
                    .FirstOrDefaultAsync(
                        o => o.OrderId == orderEvent.OrderId,
                        cancellationToken);

            if (orderEvent.EventType == "order-created")
            {
                if (order != null)
                    return;

                db.Orders.Add(
                    new OrderProjection
                    {
                        OrderId = orderEvent.OrderId,
                        Status = 0
                    });

                state.TotalOrders++;
                state.CreatedOrders++;
            }
            else if (
                orderEvent.EventType == "order-confirmed")
            {
                if (order == null)
                    return;

                if (order.Status == 0)
                    state.CreatedOrders--;
                else if (order.Status == 2)
                    state.RejectedOrders--;

                if (order.Status != 1)
                    state.ConfirmedOrders++;

                order.Status = 1;
            }
            else if (
                orderEvent.EventType == "order-rejected")
            {
                if (order == null)
                    return;

                if (order.Status == 0)
                    state.CreatedOrders--;
                else if (order.Status == 1)
                    state.ConfirmedOrders--;

                if (order.Status != 2)
                    state.RejectedOrders++;

                order.Status = 2;
            }
        }


        private static async Task HandleInventoryEventAsync(
            AnalyticsDbContext db,
            InventoryEvent inventoryEvent,
            CancellationToken cancellationToken)
        {
            var state =
                await GetStateAsync(
                    db,
                    cancellationToken);

            if (
                inventoryEvent.EventType ==
                "inventory-reserved")
            {
                state.InventoryReservedEvents++;

                state.TotalReservedQuantity +=
                    inventoryEvent.Quantity;
            }
            else if (
                inventoryEvent.EventType ==
                "inventory-rejected")
            {
                state.InventoryRejectedEvents++;
            }
        }
    }
}
