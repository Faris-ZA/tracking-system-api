using WebApplication2.DTOs.People;
using WebApplication2.Services.Caching.Interfaces;

namespace WebApplication2.Services.Caching.Implementations
{
    public class PeopleCacheService : IPeopleCacheService
    {
        private const string PeopleIdsKey =
            "people:ids";

        private const string AssignedPeopleIdsKey =
            "people:assigned";

        private const string UnassignedPeopleIdsKey =
            "people:unassigned";

        private readonly IRedisCacheService _redisCacheService;

        public PeopleCacheService(
            IRedisCacheService redisCacheService)
        {
            _redisCacheService = redisCacheService;
        }

        public async Task<PersonResponseDto?> GetByIdAsync(
            int personId)
        {
            var key =
                GetPersonKey(personId);

            return await _redisCacheService
                .GetAsync<PersonResponseDto>(key);
        }

        public async Task<PersonResponseDto?> GetByNameAsync(
            string personName)
        {
            var normalizedName =
                NormalizeName(personName);

            var nameKey =
                GetPersonNameKey(normalizedName);

            var personId =
                await _redisCacheService
                    .GetAsync<string>(nameKey);

            if (string.IsNullOrWhiteSpace(personId) ||
                !int.TryParse(personId, out var id))
            {
                return null;
            }

            return await GetByIdAsync(id);
        }

        public async Task SetAsync(
            PersonResponseDto person)
        {
            var key =
                GetPersonKey(person.Id);

            var nameKey =
                GetPersonNameKey(
                    NormalizeName(person.Name));

            await _redisCacheService
                .SetAsync(
                    key,
                    person);

            await _redisCacheService
                .SetAsync(
                    nameKey,
                    person.Id.ToString());

            await _redisCacheService
                .AddToSortedSetAsync(
                    PeopleIdsKey,
                    person.Id.ToString(),
                    person.Id);

            if (person.AssociatedTag != null)
            {
                await _redisCacheService
                    .AddToSortedSetAsync(
                        AssignedPeopleIdsKey,
                        person.Id.ToString(),
                        person.Id);

                await _redisCacheService
                    .RemoveFromSortedSetAsync(
                        UnassignedPeopleIdsKey,
                        person.Id.ToString());
            }
            else
            {
                await _redisCacheService
                    .AddToSortedSetAsync(
                        UnassignedPeopleIdsKey,
                        person.Id.ToString(),
                        person.Id);

                await _redisCacheService
                    .RemoveFromSortedSetAsync(
                        AssignedPeopleIdsKey,
                        person.Id.ToString());
            }
        }

        public async Task SetBatchAsync(
            List<PersonResponseDto> people)
        {
            const int redisBatchSize = 250;

            for (var i = 0;
                 i < people.Count;
                 i += redisBatchSize)
            {
                var batch =
                    people
                        .Skip(i)
                        .Take(redisBatchSize);

                var tasks =
                    new List<Task>();

                foreach (var person in batch)
                {
                    var key =
                        GetPersonKey(person.Id);

                    var nameKey =
                        GetPersonNameKey(
                            NormalizeName(person.Name));

                    tasks.Add(
                        _redisCacheService.SetAsync(
                            key,
                            person));

                    tasks.Add(
                        _redisCacheService.SetAsync(
                            nameKey,
                            person.Id.ToString()));

                    tasks.Add(
                        _redisCacheService.AddToSortedSetAsync(
                            PeopleIdsKey,
                            person.Id.ToString(),
                            person.Id));

                    if (person.AssociatedTag != null)
                    {
                        tasks.Add(
                            _redisCacheService.AddToSortedSetAsync(
                                AssignedPeopleIdsKey,
                                person.Id.ToString(),
                                person.Id));

                        tasks.Add(
                            _redisCacheService.RemoveFromSortedSetAsync(
                                UnassignedPeopleIdsKey,
                                person.Id.ToString()));
                    }
                    else
                    {
                        tasks.Add(
                            _redisCacheService.AddToSortedSetAsync(
                                UnassignedPeopleIdsKey,
                                person.Id.ToString(),
                                person.Id));

                        tasks.Add(
                            _redisCacheService.RemoveFromSortedSetAsync(
                                AssignedPeopleIdsKey,
                                person.Id.ToString()));
                    }
                }

                await Task.WhenAll(tasks);
            }
        }

        public async Task RemoveAsync(
            int personId)
        {
            var person =
                await GetByIdAsync(personId);

            var key =
                GetPersonKey(personId);

            await _redisCacheService
                .RemoveAsync(key);

            if (person != null)
            {
                var nameKey =
                    GetPersonNameKey(
                        NormalizeName(person.Name));

                await _redisCacheService
                    .RemoveAsync(nameKey);
            }

            await _redisCacheService
                .RemoveFromSortedSetAsync(
                    PeopleIdsKey,
                    personId.ToString());

            await _redisCacheService
                .RemoveFromSortedSetAsync(
                    AssignedPeopleIdsKey,
                    personId.ToString());

            await _redisCacheService
                .RemoveFromSortedSetAsync(
                    UnassignedPeopleIdsKey,
                    personId.ToString());
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
                        PeopleIdsKey,
                        start,
                        stop);

            return values
                .Select(int.Parse)
                .ToList();
        }

        public async Task<List<PersonResponseDto>> GetPageAsync(
            int pageNumber,
            int pageSize)
        {
            return await GetPageFromIndexAsync(
                PeopleIdsKey,
                pageNumber,
                pageSize);
        }

        public async Task<long> GetCountAsync()
        {
            return await _redisCacheService
                .GetSortedSetCountAsync(
                    PeopleIdsKey);
        }

        public async Task<List<PersonResponseDto>>
            GetAssignedPageAsync(
                int pageNumber,
                int pageSize)
        {
            return await GetPageFromIndexAsync(
                AssignedPeopleIdsKey,
                pageNumber,
                pageSize);
        }

        public async Task<List<PersonResponseDto>>
            GetUnassignedPageAsync(
                int pageNumber,
                int pageSize)
        {
            return await GetPageFromIndexAsync(
                UnassignedPeopleIdsKey,
                pageNumber,
                pageSize);
        }

        public async Task<long> GetAssignedCountAsync()
        {
            return await _redisCacheService
                .GetSortedSetCountAsync(
                    AssignedPeopleIdsKey);
        }

        public async Task<long> GetUnassignedCountAsync()
        {
            return await _redisCacheService
                .GetSortedSetCountAsync(
                    UnassignedPeopleIdsKey);
        }

        private async Task<List<PersonResponseDto>>
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

            var people =
                await Task.WhenAll(tasks);

            return people
                .Where(person => person != null)
                .Select(person => person!)
                .ToList();
        }

        private static string GetPersonKey(
            int personId)
        {
            return $"person:{personId}";
        }

        private static string GetPersonNameKey(
            string normalizedName)
        {
            return $"people:name:{normalizedName}";
        }

        private static string NormalizeName(
            string name)
        {
            return name
                .Trim()
                .ToLowerInvariant();
        }
    }
}