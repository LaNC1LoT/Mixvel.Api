namespace Mixvel.Infrastructure.OneProvider.Options;

internal sealed record class ProviderOneOptions
{
    public required string BaseAddress { get; init; } = null!;
    public int TimeoutInMls { get; init; }
}
