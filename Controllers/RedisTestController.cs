using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RedisTestController : ControllerBase
    {
        private readonly IDistributedCache _cache;

        public RedisTestController(
            IDistributedCache cache)
        {
            _cache = cache;
        }

        [HttpGet]
        public async Task<IActionResult> Test()
        {
            const string key = "redis-test";
            const string value = "Redis is connected";

            await _cache.SetStringAsync(
                key,
                value);

            var cachedValue =
                await _cache.GetStringAsync(key);

            return Ok(new
            {
                Message = cachedValue
            });
        }
    }
}