using Microsoft.Extensions.DependencyInjection;
using WebApplication2.Services.Interfaces;

namespace WebApplication2.BackgroundServices
{
    public class CacheWarmupService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<CacheWarmupService> _logger;

        public CacheWarmupService(
            IServiceScopeFactory scopeFactory,
            ILogger<CacheWarmupService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            try
            {
                _logger.LogInformation(
                    "Cache warm-up started.");

                using var scope =
                    _scopeFactory.CreateScope();

                var personService =
                    scope.ServiceProvider
                        .GetRequiredService<IPersonService>();

                var tagService =
                    scope.ServiceProvider
                        .GetRequiredService<ITagService>();

                await personService
                    .WarmCacheAsync(stoppingToken);

                await tagService
                    .WarmCacheAsync(stoppingToken);

                _logger.LogInformation(
                    "Cache warm-up completed.");
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation(
                    "Cache warm-up was cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Cache warm-up failed. The API will continue running.");
            }
        }
    }
}