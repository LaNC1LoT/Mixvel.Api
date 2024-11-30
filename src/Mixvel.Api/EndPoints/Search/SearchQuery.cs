namespace Mixvel.Api.EndPoints.Search;

internal sealed record SearchQuery
{
    public string Origin { get; init; } = null!;
    public string Destination { get; init; } = null!;
    public DateTime OriginDateTime { get; init; }
    public DateTime? DestinationDateTime { get; init; }
    public decimal? MaxPrice { get; init; }
    public DateTime? MinTimeLimit { get; init; }
    public bool? OnlyCached { get; init; }
}