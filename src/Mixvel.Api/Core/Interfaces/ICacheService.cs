namespace Mixvel.Api.Core.Interfaces;

public interface ICacheService
{
    void Set<TItem>(string key, TItem value, TimeSpan absoluteExpirationRelativeToNow);
    bool TryGetValue<TItem>(string key, out TItem? value);
}
