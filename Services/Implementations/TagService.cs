using WebApplication2.Services.Caching.Interfaces;
using WebApplication2.Constants;
using WebApplication2.DTOs.Tags;
using WebApplication2.Exceptions;
using WebApplication2.Models;
using WebApplication2.Repositories.Interfaces;
using WebApplication2.Services.Interfaces;
using System.Diagnostics;
using WebApplication2.DTOs.Performance;
using System.Text.RegularExpressions;

namespace WebApplication2.Services.Implementations
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _tagRepository;
        private readonly ITagCacheService _tagCacheService;

        public TagService(
            ITagRepository tagRepository,
            ITagCacheService tagCacheService)
        {
            _tagRepository = tagRepository;
            _tagCacheService = tagCacheService;
        }

        public async Task<List<TagResponseDto>> GetAllAsync()
        {
            var tags =
                await _tagRepository.GetAllAsync();

            return tags
                .Select(MapToResponseDto)
                .ToList();
        }

        public async Task<TagResponseDto> GetByIdAsync(
            int id)
        {
            var tag =
                await _tagRepository.GetByIdAsync(id);

            if (tag == null)
            {
                throw new NotFoundException(
                    ErrorMessages.TagNotFound);
            }

            return MapToResponseDto(tag);
        }

        public async Task<TagResponseDto> CreateAsync(
            CreateTagDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Label))
            {
                throw new BadRequestException(
                    ErrorMessages.TagLabelRequired);
            }

            if (string.IsNullOrWhiteSpace(dto.Mac))
            {
                throw new BadRequestException(
                    ErrorMessages.TagMacAddressRequired);
            }

            var label = dto.Label.Trim();
            var mac = dto.Mac.Trim();
            ValidateMacAddress(mac);

            var duplicateExists =
                await _tagRepository
                    .ActiveMacExistsAsync(mac);

            if (duplicateExists)
            {
                throw new ConflictException(
                    ErrorMessages.TagMacAddressAlreadyExists);
            }

            var currentTime = DateTime.UtcNow;

            var tag = new Tag
            {
                Label = label,
                Mac = mac,
                UpdateStatus = UpdateStatus.New,
                CreateDate = currentTime,
                LastUpdate = currentTime
            };

            await _tagRepository.AddAsync(tag);
            await _tagRepository.SaveChangesAsync();

            var response = MapToResponseDto(tag);

            try
            {
                await _tagCacheService.SetAsync(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"{CacheMessages.TagCreateFailed} {ex.Message}");
            }

            return response;
        }

        public async Task<TagResponseDto> UpdateAsync(
            int id,
            UpdateTagDto dto)
        {
            var tag =
                await _tagRepository.GetByIdAsync(id);

            if (tag == null)
            {
                throw new NotFoundException(
                    ErrorMessages.TagNotFound);
            }

            if (string.IsNullOrWhiteSpace(dto.Label))
            {
                throw new BadRequestException(
                    ErrorMessages.TagLabelRequired);
            }

            var oldLabel = tag.Label;

            tag.Label = dto.Label.Trim();

            tag.UpdateStatus =
                UpdateStatus.Updated;
            tag.LastUpdate = DateTime.UtcNow;

            await _tagRepository.SaveChangesAsync();

            var response = MapToResponseDto(tag);

            try
            {
                await _tagCacheService.UpdateLabelAsync(response, oldLabel);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"{CacheMessages.TagUpdateFailed} {ex.Message}");
            }

            return response;
        }

        public async Task<PerformancePageResponseDto<TagResponseDto>>
            GetDatabasePerformanceAsync(
                TagPerformanceQueryDto queryDto)
        {
            ValidatePerformanceQuery(queryDto);

            var totalStopwatch = Stopwatch.StartNew();

            var databaseResult =
                await _tagRepository
                    .GetPerformancePageAsync(queryDto);

            var items = databaseResult.Items
                .Select(MapToResponseDto)
                .ToList();

            totalStopwatch.Stop();

            return new PerformancePageResponseDto<TagResponseDto>
            {
                Source = PerformanceSources.Database,
                PageNumber = queryDto.PageNumber,
                PageSize = queryDto.PageSize,
                TotalRecords = databaseResult.TotalRecords,
                ReturnedRecords = items.Count,
                DatabaseQueryTimeMs =
                    databaseResult.DatabaseQueryTimeMs,
                TotalExecutionTimeMs =
                    totalStopwatch.ElapsedMilliseconds,
                Items = items
            };
        }

        public async Task DeleteAsync(int id)
        {
            var tag =
                await _tagRepository.GetByIdAsync(id);

            if (tag == null)
            {
                throw new NotFoundException(
                    ErrorMessages.TagNotFound);
            }

            var hasAssociation =
                await _tagRepository
                    .HasPeopleAssociationAsync(id);

            if (hasAssociation)
            {
                throw new ConflictException(
                    ErrorMessages.TagAssociatedWithPerson);
            }

            tag.UpdateStatus =
                UpdateStatus.Deleted;
            tag.LastUpdate = DateTime.UtcNow;

            await _tagRepository.SaveChangesAsync();

            try
            {
                await _tagCacheService.RemoveAsync(id);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"{CacheMessages.TagDeleteFailed} {ex.Message}");
            }

        }

    private static void ValidateMacAddress(string mac)
        {
            var isValid = Regex.IsMatch(
                mac,
                @"^(?:[0-9A-Fa-f]{2}([:-]))(?:[0-9A-Fa-f]{2}\1){4}[0-9A-Fa-f]{2}$|^[0-9A-Fa-f]{12}$|^(?:[0-9A-Fa-f]{4}\.){2}[0-9A-Fa-f]{4}$");

            if (!isValid)
            {
                throw new BadRequestException(
                    ErrorMessages.InvalidMacAddress);
            }
        }

        
    public async Task<PerformancePageResponseDto<TagResponseDto>>
        GetCachePerformanceAsync(
            TagPerformanceQueryDto queryDto)
    {
        try
        {
            var result =
                await GetCachePerformanceCoreAsync(queryDto);

            var skippedRecords =
                (queryDto.PageNumber - 1)
                * queryDto.PageSize;

            var remainingRecords =
                Math.Max(
                    0,
                    result.TotalRecords - skippedRecords);

            var expectedRecords =
                Math.Min(
                    queryDto.PageSize,
                    remainingRecords);

            var cacheLooksIncomplete =
                result.TotalRecords == 0
                || result.ReturnedRecords < expectedRecords;

            if (cacheLooksIncomplete)
            {
                Console.WriteLine(
                    CacheMessages.TagCacheIncomplete);

                var fallback =
                    await GetDatabasePerformanceAsync(queryDto);

                fallback.Source = PerformanceSources.DatabaseFallback;

                return fallback;
            }

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"{CacheMessages.TagCacheFailed} {ex.Message}");

            var fallback =
                await GetDatabasePerformanceAsync(queryDto);

            fallback.Source = PerformanceSources.DatabaseFallback;

            return fallback;
        }
    }

    private async Task<PerformancePageResponseDto<TagResponseDto>>
        GetCachePerformanceCoreAsync(
            TagPerformanceQueryDto queryDto)
        {
            ValidatePerformanceQuery(queryDto);

            var totalStopwatch =
                Stopwatch.StartNew();

            var cacheStopwatch =
                Stopwatch.StartNew();

            List<TagResponseDto> tags;
            long totalRecords;

            if (queryDto.TagId.HasValue)
            {
                var tag =
                    await _tagCacheService
                        .GetByIdAsync(
                            queryDto.TagId.Value);

                if (tag != null &&
                    !string.IsNullOrWhiteSpace(
                        queryDto.TagLabel) &&
                    !tag.Label.Equals(
                        queryDto.TagLabel.Trim(),
                        StringComparison.OrdinalIgnoreCase))
                {
                    tag = null;
                }

                if (tag != null &&
                    !MatchesAssignmentStatus(
                        tag,
                        queryDto.IsAssigned))
                {
                    tag = null;
                }

                tags =
                    tag == null
                        ? new List<TagResponseDto>()
                        : new List<TagResponseDto>
                        {
                            tag
                        };

                totalRecords =
                    tags.Count;
            }
            else if (!string.IsNullOrWhiteSpace(
                queryDto.TagLabel))
            {
                tags =
                    await _tagCacheService
                        .GetByLabelAsync(
                            queryDto.TagLabel,
                            queryDto.PageNumber,
                            queryDto.PageSize);

                if (queryDto.IsAssigned.HasValue)
                {
                    tags =
                        tags
                            .Where(tag =>
                                MatchesAssignmentStatus(
                                    tag,
                                    queryDto.IsAssigned))
                            .ToList();
                }

                totalRecords =
                    tags.Count;
            }
            else if (
                queryDto.IsAssigned == true)
            {
                tags =
                    await _tagCacheService
                        .GetAssignedPageAsync(
                            queryDto.PageNumber,
                            queryDto.PageSize);

                totalRecords =
                    await _tagCacheService
                        .GetAssignedCountAsync();
            }
            else if (
                queryDto.IsAssigned == false)
            {
                tags =
                    await _tagCacheService
                        .GetUnassignedPageAsync(
                            queryDto.PageNumber,
                            queryDto.PageSize);

                totalRecords =
                    await _tagCacheService
                        .GetUnassignedCountAsync();
            }
            else
            {
                tags =
                    await _tagCacheService
                        .GetPageAsync(
                            queryDto.PageNumber,
                            queryDto.PageSize);

                totalRecords =
                    await _tagCacheService
                        .GetCountAsync();
            }

            cacheStopwatch.Stop();
            totalStopwatch.Stop();

            return new PerformancePageResponseDto<TagResponseDto>
            {
                Source = PerformanceSources.Cache,

                PageNumber =
                    queryDto.PageNumber,

                PageSize =
                    queryDto.PageSize,

                TotalRecords =
                    (int)totalRecords,

                ReturnedRecords =
                    tags.Count,

                DatabaseQueryTimeMs = 0,

                CacheQueryTimeMs =
                    cacheStopwatch.ElapsedMilliseconds,

                TotalExecutionTimeMs =
                    totalStopwatch.ElapsedMilliseconds,

                Items = tags
            };
        }

        private static bool MatchesAssignmentStatus(
            TagResponseDto tag,
            bool? isAssigned)
        {
            if (!isAssigned.HasValue)
            {
                return true;
            }

            return isAssigned.Value
                ? tag.AssociatedPerson != null
                : tag.AssociatedPerson == null;
        }

        private static void ValidatePerformanceQuery(
            TagPerformanceQueryDto queryDto)
        {
            if (queryDto.PageNumber < 1)
            {
                throw new BadRequestException(
                    ErrorMessages.InvalidPageNumber);
            }

            if (queryDto.PageSize < 1 ||
                queryDto.PageSize > 500)
            {
                throw new BadRequestException(
                    ErrorMessages.InvalidPageSize);
            }
        }


        private static TagResponseDto MapToResponseDto(
            Tag tag)
        {
            var activeAssociation = tag.PeopleAssociations
                .FirstOrDefault(a => 
                a.Person.UpdateStatus != UpdateStatus.Deleted);

            return new TagResponseDto
            {
                Id = tag.Id,
                Label = tag.Label,
                Mac = tag.Mac,
                CreateDate = tag.CreateDate,
                LastUpdate = tag.LastUpdate,

                AssociatedPerson = activeAssociation == null 
                ? null
                :
                new AssociatedPersonDto
                {
                    Id = activeAssociation.Person.Id,
                    Name = activeAssociation.Person.Name
                }
            };
        }

        public async Task WarmCacheAsync(
           CancellationToken cancellationToken)
        {
            const int batchSize = 5000;

            var skip = 0;

            while (!cancellationToken.IsCancellationRequested)
            {
                var tags =
                    await _tagRepository.GetBatchAsync(
                        skip,
                        batchSize);

                if (tags.Count == 0)
                {
                    break;
                }

                cancellationToken.ThrowIfCancellationRequested();

                var tagDtos =
                    tags
                        .Select(MapToResponseDto)
                        .ToList();

                await _tagCacheService
                    .SetBatchAsync(tagDtos);

                if (tags.Count < batchSize)
                {
                    break;
                }

                skip += batchSize;
            }
        }
    }
}









