using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace RedisTest;

class RedisDemo
{
    ILogger<RedisDemo> _logger;
    IDistributedCache _cache;

    public RedisDemo(ILogger<RedisDemo> logger, IDistributedCache cache)
    {
        _logger = logger;
        _cache = cache;
    }

    public async Task AddToCache<T>(string key, T data)
    {
        _logger.LogDebug("AddToCache");

        var options = new DistributedCacheEntryOptions();

        options.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(120);
        options.SlidingExpiration = TimeSpan.FromSeconds(10);

         var jsonData = JsonSerializer.Serialize(data);
         await _cache.SetStringAsync(key, jsonData, options);



    }

    public async Task<T?> ReadFromCache<T>(string key)
    {
        _logger.LogDebug("ReadFromCache");

        var jsonData = await _cache.GetStringAsync(key);

        if (jsonData is null)
        {
            return default(T);
        }

        return JsonSerializer.Deserialize<T>(jsonData);
    }
}