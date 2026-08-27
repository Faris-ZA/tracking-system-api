using AnalyticsService.Clients;
using AnalyticsService.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient<ICustomerClient, CustomerClient>(
    client =>
    {
        client.BaseAddress =
            new Uri(
                builder.Configuration[
                    "Services:CustomerService"]!);
    });

builder.Services.AddHttpClient<IOrderClient, OrderClient>(
    client =>
    {
        client.BaseAddress =
            new Uri(
                builder.Configuration[
                    "Services:OrderService"]!);
    });

builder.Services.AddHttpClient<IInventoryClient, InventoryClient>(
    client =>
    {
        client.BaseAddress =
            new Uri(
                builder.Configuration[
                    "Services:InventoryService"]!);
    });

builder.Services.AddScoped<
    IAnalyticsService,
    AnalyticsService.Services.AnalyticsService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
