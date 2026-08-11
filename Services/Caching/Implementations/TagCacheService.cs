using WebApplication2.DTOs.Tags;
using WebApplication2.Services.Caching.Interfaces;

namespace WebApplication2.Services.Caching.Implementations
{
    public class TagCacheService : ITagCacheService
    {
        private const string TagsIdsKey =
            "tags:ids";

        private const string AssignedTagsIdsKey =
            "tags:assigned";

        private const string UnassignedTagsIdsKey =
            "tags:unassigned";

        private readonly IRedisCacheService _redisCacheService;

        public TagCacheService(
            IRedisCacheService redisCacheService)
        {
            _redisCacheService = redisCacheService;
        }

        public async Task<TagResponseDto?> GetByIdAsync(
            int tagId)
        {
            var key =
                GetTagKey(tagId);

            return await _redisCacheService
                .GetAsync<TagResponseDto>(key);
        }

        public async Task SetAsync(
            TagResponseDto tag)
        {
            var key =
                GetTagKey(tag.Id);

            var labelKey =
                GetTagLabelKey(
                    NormalizeLabel(tag.Label));

            await _redisCacheService
                .SetAsync(
                    key,
                    tag);

            await _redisCacheService
                .AddToSortedSetAsync(
                    TagsIdsKey,
                    tag.Id.ToString(),
                    tag.Id);

            await _redisCacheService
                .AddToSortedSetAsync(
                    labelKey,
                    tag.Id.ToString(),
                    tag.Id);

            if (tag.AssociatedPerson != null)
            {
                await _redisCacheService
                    .AddToSortedSetAsync(
                        AssignedTagsIdsKey,
                        tag.Id.ToString(),
                        tag.Id);

                await _redisCacheService
                    .RemoveFromSortedSetAsync(
                        UnassignedTagsIdsKey,
                        tag.Id.ToString());
            }
            else
            {
                await _redisCacheService
                    .AddToSortedSetAsync(
                        UnassignedTagsIdsKey,
                        tag.Id.ToString(),
                        tag.Id);

                await _redisCacheService
                    .RemoveFromSortedSetAsync(
                        AssignedTagsIdsKey,
                        tag.Id.ToString());
            }
        }

        public async Task UpdateLabelAsync(
            TagResponseDto tag,
            string oldLabel)
        {
            var oldLabelKey =
                GetTagLabelKey(
                    NormalizeLabel(oldLabel));

            await _redisCacheService
                .RemoveFromSortedSetAsync(
                    oldLabelKey,
                    tag.Id.ToString());

            await SetAsync(tag);
        }

        public async Task SetBatchAsync(
            List<TagResponseDto> tags)
        {
            var tasks =
                new List<Task>();

            foreach (var tag in tags)
            {
                var key =
                    GetTagKey(tag.Id);

                var labelKey =
                    GetTagLabelKey(
                        NormalizeLabel(tag.Label));

                tasks.Add(
                    _redisCacheService.SetAsync(
                        key,
                        tag));

                tasks.Add(
                    _redisCacheService.AddToSortedSetAsync(
                        TagsIdsKey,
                        tag.Id.ToString(),
                        tag.Id));

                tasks.Add(
                    _redisCacheService.AddToSortedSetAsync(
                        labelKey,
                        tag.Id.ToString(),
                        tag.Id));

                if (tag.AssociatedPerson != null)
                {
                    tasks.Add(
                        _redisCacheService.AddToSortedSetAsync(
                            AssignedTagsIdsKey,
                            tag.Id.ToString(),
                            tag.Id));
                }
                else
                {
                    tasks.Add(
                        _redisCacheService.AddToSortedSetAsync(
                            UnassignedTagsIdsKey,
                            tag.Id.ToString(),
                            tag.Id));
                }
            }

            await Task.WhenAll(tasks);
        }

        public async Task RemoveAsync(
            int tagId)
        {
            var tag =
                await GetByIdAsync(tagId);

            var key =
                GetTagKey(tagId);

            await _redisCacheService
                .RemoveAsync(key);

            if (tag != null)
            {
                var labelKey =
                    GetTagLabelKey(
                        NormalizeLabel(tag.Label));

                await _redisCacheService
                    .RemoveFromSortedSetAsync(
                        labelKey,
                        tagId.ToString());
            }

            await _redisCacheService
                .RemoveFromSortedSetAsync(
                    TagsIdsKey,
                    tagId.ToString());

            await _redisCacheService
                .RemoveFromSortedSetAsync(
                    AssignedTagsIdsKey,
                    tagId.ToString());

            await _redisCacheService
                .RemoveFromSortedSetAsync(
                    UnassignedTagsIdsKey,
                    tagId.ToString());
        }

        public async Task<List<int>> GetPageIdsAsync(
            int pageNumber,
            int pageSize)
        {
            var start =
                (pageNumber - 1) * pageSize;

            var stop =
                start + pageSize - 1;

            var values =
                await _redisCacheService
                    .GetSortedSetRangeAsync(
                        TagsIdsKey,
                        start,
                        stop);

            return values
                .Select(int.Parse)
                .ToList();
        }

        public async Task<List<TagResponseDto>> GetPageAsync(
            int pageNumber,
            int pageSize)
        {
            return await GetPageFromIndexAsync(
                TagsIdsKey,
                pageNumber,
                pageSize);
        }

        public async Task<long> GetCountAsync()
        {
            return await _redisCacheService
                .GetSortedSetCountAsync(
                    TagsIdsKey);
        }

        public async Task<List<TagResponseDto>>
            GetAssignedPageAsync(
                int pageNumber,
                int pageSize)
        {
            return await GetPageFromIndexAsync(
                AssignedTagsIdsKey,
                pageNumber,
                pageSize);
        }

        public async Task<List<TagResponseDto>>
            GetUnassignedPageAsync(
                int pageNumber,
                int pageSize)
        {
            return await GetPageFromIndexAsync(
                UnassignedTagsIdsKey,
                pageNumber,
                pageSize);
        }

        public async Task<long> GetAssignedCountAsync()
        {
            return await _redisCacheService
                .GetSortedSetCountAsync(
                    AssignedTagsIdsKey);
        }

        public async Task<long> GetUnassignedCountAsync()
        {
            return await _redisCacheService
                .GetSortedSetCountAsync(
                    UnassignedTagsIdsKey);
        }

        public async Task<List<TagResponseDto>> GetByLabelAsync(
            string tagLabel,
            int pageNumber,
            int pageSize)
        {
            var labelKey =
                GetTagLabelKey(
                    NormalizeLabel(tagLabel));

            return await GetPageFromIndexAsync(
                labelKey,
                pageNumber,
                pageSize);
        }

        public async Task<long> GetLabelCountAsync(
            string tagLabel)
        {
            var labelKey =
                GetTagLabelKey(
                    NormalizeLabel(tagLabel));

            return await _redisCacheService
                .GetSortedSetCountAsync(
                    labelKey);
        }

        private async Task<List<TagResponseDto>>
            GetPageFromIndexAsync(
                string indexKey,
                int pageNumber,
                int pageSize)
        {
            var start =
                (pageNumber - 1) * pageSize;

            var stop =
                start + pageSize - 1;

            var values =
                await _redisCacheService
                    .GetSortedSetRangeAsync(
                        indexKey,
                        start,
                        stop);

            var ids =
                values
                    .Select(int.Parse)
                    .ToList();

            var tasks =
                ids.Select(GetByIdAsync);

            var tags =
                await Task.WhenAll(tasks);

            return tags
                .Where(tag => tag != null)
                .Select(tag => tag!)
                .ToList();
        }

        private static string GetTagKey(
            int tagId)
        {
            return $"tag:{tagId}";
        }

        private static string GetTagLabelKey(
            string normalizedLabel)
        {
            return $"tags:label:{normalizedLabel}";
        }

        private static string NormalizeLabel(
            string label)
        {
            return label
                .Trim()
                .ToLowerInvariant();
        }
    }
}