using Microsoft.Extensions.Options;
using Mixvel.Api.Core.Interfaces;
using Mixvel.Api.Core.Models;
using Mixvel.Api.Options;
using Route = Mixvel.Api.Core.Models.Route;

namespace Mixvel.Api.Infrastructure.Aggregators;

public sealed class AggregatedSearchService(IEnumerable<IProviderAdapter> providers,
    ICacheService cache,
    IOptions<CacheOptions> options,
    ILogger<AggregatedSearchService> logger) : ISearchService
{
    public async Task<SearchResponse> SearchAsync(SearchRequest request, CancellationToken cancellationToken)
    {
        if (request.Filters?.OnlyCached == true)
        {
            return TryGetCached(request);
        }

        var tasks = providers.Select(provider => provider.SearchAsync(request, cancellationToken));
        var results = await Task.WhenAll(tasks);

        var aggregatedRoutes = results.SelectMany(s => s);

        var cacheKey = GenerateCacheKey(request);
        cache.Set(cacheKey, aggregatedRoutes, TimeSpan.FromMilliseconds(options.Value.CacheRouteLifeTimeInMls));

        return ApplyFilters(request, aggregatedRoutes);
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
    {
        var tasks = providers.Select(provider => provider.IsAvailableAsync(cancellationToken));
        var results = await Task.WhenAll(tasks);
        return results.Any(r => r);
    }

    private static SearchResponse ApplyFilters(SearchRequest request, IEnumerable<Route>? routes)
    {
        if (request.Filters is null || routes is null)
        {
            return new SearchResponse();
        }

        var filteredRoutes = routes
            .Where(r => !request.Filters.DestinationDateTime.HasValue || r.DestinationDateTime <= request.Filters.DestinationDateTime.Value)
            .Where(r => !request.Filters.MaxPrice.HasValue || r.Price <= request.Filters.MaxPrice.Value)
            .Where(r => !request.Filters.MinTimeLimit.HasValue || r.TimeLimit >= request.Filters.MinTimeLimit.Value)
            .ToArray();

        if (filteredRoutes.Length == 0)
        {
            return new SearchResponse();
        }

        return new SearchResponse
        {
            Routes = filteredRoutes,
            MinPrice = filteredRoutes.Min(r => r.Price),
            MaxPrice = filteredRoutes.Max(r => r.Price),
            MinMinutesRoute = filteredRoutes.Min(r => (int)(r.DestinationDateTime - r.OriginDateTime).TotalMinutes),
            MaxMinutesRoute = filteredRoutes.Max(r => (int)(r.DestinationDateTime - r.OriginDateTime).TotalMinutes)
        };
    }

    private SearchResponse TryGetCached(SearchRequest request)
    {
        var cacheKey = GenerateCacheKey(request);
        if (cache.TryGetValue(cacheKey, out Route[]? routes))
        {
            logger.LogInformation("Returning cached results.");
            return ApplyFilters(request, routes);
        }
        else
        {
            logger.LogWarning("No cached data found.");
            return new SearchResponse();
        }
    }

    private static string GenerateCacheKey(SearchRequest request)
    {
        return $"{request.Origin}_{request.Destination}_{request.OriginDateTime}";
    }
}
