using Microsoft.EntityFrameworkCore;
using Npgsql.NameTranslation;
using WebApplication2.BackgroundServices;
using WebApplication2.Configuration;
using WebApplication2.Data;
using WebApplication2.Infrastructure.Email;
using WebApplication2.Infrastructure.Export;
using WebApplication2.Infrastructure.Import;
using WebApplication2.Middleware;
using WebApplication2.Models;
using WebApplication2.Repositories.Implementations;
using WebApplication2.Repositories.Interfaces;
using WebApplication2.Services.Implementations;
using WebApplication2.Services.ImportParsers.Interfaces;
using WebApplication2.Services.Interfaces;
using WebApplication2.Services.Caching.Implementations;
using WebApplication2.Services.Caching.Interfaces;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Connection string
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "The DefaultConnection connection string was not found.");


var enumNameTranslator =
    new NpgsqlNullNameTranslator();

// PostgreSQL + EF Core
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        connectionString,
        npgsqlOptions =>
        {
            npgsqlOptions.MapEnum<UpdateStatus>(
                "update_status",
                nameTranslator: enumNameTranslator); ;
        }));

// Redis distributed cache
var redisConnectionString =
    builder.Configuration["Redis:ConnectionString"]
    ?? throw new InvalidOperationException(
        "Redis connection string was not found.");

var redisOptions =
    ConfigurationOptions.Parse(redisConnectionString);

redisOptions.AbortOnConnectFail = false;

builder.Services.AddSingleton<IConnectionMultiplexer>(
    ConnectionMultiplexer.Connect(redisOptions));

builder.Services.AddScoped<IVenueRepository, VenueRepository>();
builder.Services.AddScoped<IVenueService, VenueService>();


builder.Services.AddScoped<IFloorRepository, FloorRepository>();
builder.Services.AddScoped<IFloorService, FloorService>();


builder.Services.AddScoped<IZoneRepository, ZoneRepository>();
builder.Services.AddScoped<IZoneService, ZoneService>();


builder.Services.AddScoped<IPersonRepository, PersonRepository>();
builder.Services.AddScoped<IPersonService, PersonService>();


builder.Services.AddScoped<ITagRepository, TagRepository>();
builder.Services.AddScoped<ITagService, TagService>();

builder.Services.AddScoped<IPeopleLocationReportRepository,PeopleLocationReportRepository>();

builder.Services.AddScoped<IPeopleTagAssociationRepository, PeopleTagAssociationRepository>();
builder.Services.AddScoped<IPeopleTagAssociationService, PeopleTagAssociationService>();

builder.Services.AddScoped<IPositionRepository, PositionRepository>();
builder.Services.AddScoped<IPositionService, PositionService>();

builder.Services.AddScoped<IPeopleLocationReportService, PeopleLocationReportService>();

builder.Services.AddScoped<ICsvExportService, CsvExportService>();

builder.Services.AddScoped< IPeopleLocationExportService, PeopleLocationExportService>();

builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<ICsvPeopleImportParser, CsvPeopleImportParser>();

builder.Services.AddScoped<IExcelPeopleImportParser, ExcelPeopleImportParser>();

builder.Services.AddScoped<
    IPeopleImportFileReader,
    PeopleImportFileReader>();
builder.Services.AddScoped<IPeopleImportService, PeopleImportService>();

builder.Services.Configure<BackgroundJobSettings>(
    builder.Configuration.GetSection(
        "BackgroundJobSettings"));

builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection(
        "EmailSettings"));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration =
        builder.Configuration["Redis:ConnectionString"];

    options.InstanceName = "TrackingSystem:";
});

builder.Services.AddHostedService<OfflinePeopleReportHostedService>();

builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();

builder.Services.AddScoped<IPeopleCacheService, PeopleCacheService>();

builder.Services.AddScoped<ITagCacheService, TagCacheService>();

builder.Services.AddHostedService<CacheWarmupService>();

builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMiddleware<GlobalExceptionMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/openapi/v1.json",
            "Tracking System API v1");

        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();




