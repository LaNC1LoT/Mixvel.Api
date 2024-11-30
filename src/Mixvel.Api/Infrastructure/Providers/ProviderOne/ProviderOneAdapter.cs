using Mixvel.Api.Core.Interfaces;
using Mixvel.Api.Core.Models;
using Route = Mixvel.Api.Core.Models.Route;

namespace Mixvel.Api.Infrastructure.Providers.ProviderOne;

internal sealed class ProviderOneAdapter(IProviderOneService provider) : IProviderAdapter
{
    public async Task<Route[]> SearchAsync(SearchRequest query, CancellationToken cancellationToken)
    {
        var request = MapToRequest(query);
        var response = await provider.SearchAsync(request, cancellationToken);
        if (response is null || response.Routes is null)
        {
            return [];
        }

        if (response.Routes.Length == 0)
        {
            return [];
        }

        return MapToResponse(response);
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
    {
        return await provider.IsAvailableAsync(cancellationToken);
    }

    public static ProviderOneSearchRequest MapToRequest(SearchRequest request)
    {
        return new ProviderOneSearchRequest
        {
            From = request.Origin,
            To = request.Destination,
            DateFrom = request.OriginDateTime,
            DateTo = request.Filters?.DestinationDateTime,
            MaxPrice = request.Filters?.MaxPrice
        };
    }

    public static Route[] MapToResponse(ProviderOneSearchResponse response)
    {
        return response.Routes.Select(s => new Route
        {
            Id = Guid.NewGuid(),
            Origin = s.From,
            Destination = s.To,
            OriginDateTime = s.DateFrom,
            DestinationDateTime = s.DateTo,
            Price = s.Price,
            TimeLimit = s.TimeLimit
        }).ToArray();
    }
}
