namespace Mixvel.Api.Options;

public sealed record class CacheOptions
{
    public int CacheRouteLifeTimeInMls { get; init; }
}