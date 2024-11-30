using Mixvel.Api.Core.Interfaces;
using Mixvel.Api.Core.Models;

namespace Mixvel.Api.EndPoints.Search;

internal sealed class SearchEndPoint : IEndPoint
{
    public void MapEndPoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/search", SearchHandler)
            .Produces<SearchResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Search");

        app.MapGet("/ping", PingHandler)
            .Produces(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status500InternalServerError)
            .WithTags("Ping");
    }

    private async Task<IResult> PingHandler(ISearchService searchService, CancellationToken cancellationToken)
    {
        var result = await searchService.IsAvailableAsync(cancellationToken);
        return result ? Results.Ok() : Results.InternalServerError();
    }

    private async Task<IResult> SearchHandler([AsParameters] SearchQuery query, ISearchService searchService, CancellationToken cancellationToken)
    {
        var request = query.MapToRequest();
        var result = await searchService.SearchAsync(request, cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result);
    }
}
