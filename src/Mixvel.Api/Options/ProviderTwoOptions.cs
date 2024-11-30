namespace Mixvel.Api.Options;

public sealed record class ProviderTwoOptions
{
    public required string BaseAddress { get; init; } = null!;
    public int TimeoutInMls { get; init; }
}

