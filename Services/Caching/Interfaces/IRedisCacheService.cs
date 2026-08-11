namespace WebApplication2.Services.Caching.Interfaces
{
    public interface IRedisCacheService
    {
        Task<T?> GetAsync<T>(string key);

        Task SetAsync<T>(
            string key,
            T value);

        Task RemoveAsync(string key);

        Task AddToSortedSetAsync(
            string key,
            string member,
            double score);

        Task RemoveFromSortedSetAsync(
            string key,
            string member);

        Task<List<string>> GetSortedSetRangeAsync(
            string key,
            long start,
            long stop);

        Task<long> GetSortedSetCountAsync(
            string key);
    }
}