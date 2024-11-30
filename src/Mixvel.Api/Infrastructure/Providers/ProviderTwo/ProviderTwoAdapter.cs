using Mixvel.Api.Core.Interfaces;
using Mixvel.Api.Core.Models;
using Route = Mixvel.Api.Core.Models.Route;

namespace Mixvel.Api.Infrastructure.Providers.ProviderTwo;

internal sealed class ProviderTwoAdapter(IProviderTwoService provider) : IProviderAdapter
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

    private static ProviderTwoSearchRequest MapToRequest(SearchRequest request)
    {
        return new ProviderTwoSearchRequest
        {
            Departure = request.Origin,
            Arrival = request.Destination,
            DepartureDate = request.OriginDateTime,
            MinTimeLimit = request.Filters?.MinTimeLimit
        };
    }

    private static Route[] MapToResponse(ProviderTwoSearchResponse response)
    {
        return response.Routes.Select(s => new Route
        {
            Id = Guid.NewGuid(),
            Origin = s.Departure.Point,
            Destination = s.Arrival.Point,
            OriginDateTime = s.Departure.Date,
            DestinationDateTime = s.Arrival.Date,
            Price = s.Price,
            TimeLimit = s.TimeLimit
        }).ToArray();
    }
}