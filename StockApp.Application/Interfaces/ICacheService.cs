using System.Threading.Tasks;

namespace StockApp.Application.Interfaces
{
    public interface ICacheService
    {
        Task<string> GetCacheValueAsync(string key);
        Task SetCacheValueAsync(string key, string value, TimeSpan timeToLive);
        Task ClearCacheAsync(string key);
    }
}