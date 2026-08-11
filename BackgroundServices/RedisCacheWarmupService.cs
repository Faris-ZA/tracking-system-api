using Microsoft.Extensions.DependencyInjection;
using WebApplication2.DTOs.People;
using WebApplication2.DTOs.Tags;
using WebApplication2.Models;
using WebApplication2.Repositories.Interfaces;
using WebApplication2.Services.Caching.Interfaces;

namespace WebApplication2.BackgroundServices
{
    public class RedisCacheWarmupService : BackgroundService
    {
        private const int BatchSize = 5000;

        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<RedisCacheWarmupService> _logger;

        public RedisCacheWarmupService(
            IServiceScopeFactory scopeFactory,
            ILogger<RedisCacheWarmupService> logger)
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
                    "Redis cache warm-up started.");

                using var scope =
                    _scopeFactory.CreateScope();

                var personRepository =
                    scope.ServiceProvider
                        .GetRequiredService<IPersonRepository>();

                var tagRepository =
                    scope.ServiceProvider
                        .GetRequiredService<ITagRepository>();

                var peopleCacheService =
                    scope.ServiceProvider
                        .GetRequiredService<IPeopleCacheService>();

                var tagCacheService =
                    scope.ServiceProvider
                        .GetRequiredService<ITagCacheService>();

                await WarmPeopleAsync(
                    personRepository,
                    peopleCacheService,
                    stoppingToken);

                await WarmTagsAsync(
                    tagRepository,
                    tagCacheService,
                    stoppingToken);

                _logger.LogInformation(
                    "Redis cache warm-up completed.");
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation(
                    "Redis cache warm-up was cancelled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Redis cache warm-up failed. The API will continue running.");
            }
        }

        private async Task WarmPeopleAsync(
            IPersonRepository personRepository,
            IPeopleCacheService peopleCacheService,
            CancellationToken stoppingToken)
        {
            var skip = 0;

            while (!stoppingToken.IsCancellationRequested)
            {
                var people =
                    await personRepository.GetBatchAsync(
                        skip,
                        BatchSize);

                if (people.Count == 0)
                {
                    break;
                }

                stoppingToken.ThrowIfCancellationRequested();

                var peopleDtos =
                    people
                        .Select(MapPersonToResponseDto)
                        .ToList();

                await peopleCacheService
                    .SetBatchAsync(peopleDtos);

                _logger.LogInformation(
                    "Cached {Count} People. Current offset: {Offset}",
                    people.Count,
                    skip);

                if (people.Count < BatchSize)
                {
                    break;
                }

                skip += BatchSize;
            }
        }

        private async Task WarmTagsAsync(
            ITagRepository tagRepository,
            ITagCacheService tagCacheService,
            CancellationToken stoppingToken)
        {
            var skip = 0;

            while (!stoppingToken.IsCancellationRequested)
            {
                var tags =
                    await tagRepository.GetBatchAsync(
                        skip,
                        BatchSize);

                if (tags.Count == 0)
                {
                    break;
                }

                stoppingToken.ThrowIfCancellationRequested();

                var tagDtos =
                    tags
                        .Select(MapTagToResponseDto)
                        .ToList();

                await tagCacheService
                    .SetBatchAsync(tagDtos);

                _logger.LogInformation(
                    "Cached {Count} Tags. Current offset: {Offset}",
                    tags.Count,
                    skip);

                if (tags.Count < BatchSize)
                {
                    break;
                }

                skip += BatchSize;
            }
        }

        private static PersonResponseDto MapPersonToResponseDto(
            Person person)
        {
            var activeAssociation =
                person.TagAssociations
                    .FirstOrDefault(a =>
                        a.Tag.UpdateStatus
                        != UpdateStatus.Deleted);

            return new PersonResponseDto
            {
                Id = person.Id,
                Name = person.Name,
                Phone = person.Phone,
                CreateDate = person.CreateDate,
                LastUpdate = person.LastUpdate,

                AssociatedTag =
                    activeAssociation == null
                        ? null
                        : new AssociatedTagDto
                        {
                            Id = activeAssociation.Tag.Id,
                            Mac = activeAssociation.Tag.Mac
                        }
            };
        }

        private static TagResponseDto MapTagToResponseDto(
            Tag tag)
        {
            var activeAssociation =
                tag.PeopleAssociations
                    .FirstOrDefault(a =>
                        a.Person.UpdateStatus
                        != UpdateStatus.Deleted);

            return new TagResponseDto
            {
                Id = tag.Id,
                Label = tag.Label,
                Mac = tag.Mac,
                CreateDate = tag.CreateDate,
                LastUpdate = tag.LastUpdate,

                AssociatedPerson =
                    activeAssociation == null
                        ? null
                        : new AssociatedPersonDto
                        {
                            Id = activeAssociation.Person.Id,
                            Name = activeAssociation.Person.Name
                        }
            };
        }
    }
}