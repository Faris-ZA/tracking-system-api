using System.Text.Json;
using Confluent.Kafka;
using KafkaOrderService.Data;
using KafkaOrderService.Events;
using KafkaOrderService.Models;
using Microsoft.EntityFrameworkCore;

namespace KafkaOrderService.Kafka
{
    public class CustomerEventsConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _scopeFactory;

        public CustomerEventsConsumer(
            IConfiguration configuration,
            IServiceScopeFactory scopeFactory)
        {
            _configuration = configuration;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                GroupId = "kafka-order-customer-group",
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            using var consumer = new ConsumerBuilder<string, string>(config)
                .Build();

            consumer.Subscribe("customer-events");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result = consumer.Consume(stoppingToken);

                    var customerEvent =
                        JsonSerializer.Deserialize<CustomerEvent>(
                            result.Message.Value);

                    if (customerEvent == null)
                        continue;

                    using var scope = _scopeFactory.CreateScope();

                    var dbContext =
                        scope.ServiceProvider.GetRequiredService<OrderDbContext>();

                    var localCustomer =
                        await dbContext.LocalCustomers
                            .FirstOrDefaultAsync(
                                c => c.CustomerId == customerEvent.CustomerId,
                                stoppingToken);

                    if (localCustomer == null)
                    {
                        localCustomer = new LocalCustomer
                        {
                            CustomerId = customerEvent.CustomerId,
                            Status = customerEvent.Status
                        };

                        dbContext.LocalCustomers.Add(localCustomer);
                    }
                    else
                    {
                        localCustomer.Status = customerEvent.Status;
                    }

                    await dbContext.SaveChangesAsync(stoppingToken);

                    consumer.Commit(result);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(
                        $"Customer event consumer error: {ex.Message}");
                }
            }

            consumer.Close();
        }
    }
}
