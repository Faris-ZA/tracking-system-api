using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using WebApplication2.Services.Caching.Interfaces;
using StackExchange.Redis;

namespace WebApplication2.Services.Caching.Implementations
{

    
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _cache;
        private readonly IDatabase _database;

        public RedisCacheService(
            IDistributedCache cache,
            IConnectionMultiplexer connectionMultiplexer)
        {
            _cache = cache;
            _database = connectionMultiplexer.GetDatabase();
        }

        public async Task<T?> GetAsync<T>(
            string key)
        {
            var json =
                await _cache.GetStringAsync(key);

            if (string.IsNullOrWhiteSpace(json))
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(json);
        }

        public async Task SetAsync<T>(
            string key,
            T value)
        {
            var json =
                JsonSerializer.Serialize(value);

            await _cache.SetStringAsync(
                key,
                json);
        }

        public async Task RemoveAsync(
            string key)
        {
            await _cache.RemoveAsync(key);
        }

        public async Task AddToSortedSetAsync(
            string key,
            string member,
            double score)
        {
            var redisKey =
                GetPrefixedKey(key);

            await _database.SortedSetAddAsync(
                redisKey,
                member,
                score);
        }

        public async Task RemoveFromSortedSetAsync(
            string key,
            string member)
        {
            var redisKey =
                GetPrefixedKey(key);

            await _database.SortedSetRemoveAsync(
                redisKey,
                member);
        }

        public async Task<List<string>> GetSortedSetRangeAsync(
            string key,
            long start,
            long stop)
        {
            var redisKey =
                GetPrefixedKey(key);

            var values =
                await _database
                    .SortedSetRangeByRankAsync(
                        redisKey,
                        start,
                        stop);

            return values
                .Select(value => value.ToString())
                .ToList();
        }

        public async Task<long> GetSortedSetCountAsync(
            string key)
        {
            var redisKey =
                GetPrefixedKey(key);

            return await _database
                .SortedSetLengthAsync(redisKey);
        }

        private static string GetPrefixedKey(
            string key)
        {
            return $"TrackingSystem:{key}";
        }
    }
}