using KafkaInventoryService.Kafka;
using KafkaInventoryService.Data;
using KafkaInventoryService.Repositories;
using KafkaInventoryService.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<IKafkaInventoryService, KafkaInventoryService.Services.KafkaInventoryService>();

builder.Services.AddSingleton<IInventoryEventProducer, InventoryEventProducer>();
builder.Services.AddHostedService<OrderEventsConsumer>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

