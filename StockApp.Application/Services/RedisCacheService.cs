using StackExchange.Redis;
using StockApp.Application.Interfaces;
using System;
using System.Threading.Tasks;

namespace StockApp.Application.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _cacheDb;

        public RedisCacheService(IConnectionMultiplexer redisConnection)
        {
            _cacheDb = redisConnection.GetDatabase();
        }

        public async Task<string> GetCacheValueAsync(string key)
        {
            return await _cacheDb.StringGetAsync(key);
        }

        public async Task SetCacheValueAsync(string key, string value, TimeSpan timeToLive)
        {
            await _cacheDb.StringSetAsync(key, value, timeToLive);
        }

        public async Task ClearCacheAsync(string key)
        {
            await _cacheDb.KeyDeleteAsync(key);
        }
    }
}