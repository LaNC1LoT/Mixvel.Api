using Microsoft.Extensions.Caching.Memory;
using Mixvel.Api.Core.Interfaces;

namespace Mixvel.Api.Infrastructure;

public class MemoryCacheService(IMemoryCache memoryCache) : ICacheService
{
    public void Set<TItem>(string key, TItem value, TimeSpan absoluteExpirationRelativeToNow)
    {
        memoryCache.Set(key, value, absoluteExpirationRelativeToNow);
    }

    public bool TryGetValue<TItem>(string key, out TItem? value)
    {
        return memoryCache.TryGetValue(key, out value);
    }
}
