using Mixvel.Api.Core.Models;

namespace Mixvel.Api.EndPoints.Search;

internal static class SearchRequestMapper
{
    public static SearchRequest MapToRequest(this SearchQuery query)
    {
        return new SearchRequest()
        {
            Origin = query.Origin,
            Destination = query.Destination,
            OriginDateTime = query.OriginDateTime,
            Filters = new SearchFilters
            {
                DestinationDateTime = query.DestinationDateTime,
                MaxPrice = query.MaxPrice,
                MinTimeLimit = query.MinTimeLimit,
                OnlyCached = query.OnlyCached
            }
        };
    }
}
