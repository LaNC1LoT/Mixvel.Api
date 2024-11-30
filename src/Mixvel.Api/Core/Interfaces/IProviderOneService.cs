using Mixvel.Api.Infrastructure.Providers.ProviderOne;

namespace Mixvel.Api.Core.Interfaces;

public interface IProviderOneService
{
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken);
    Task<ProviderOneSearchResponse?> SearchAsync(ProviderOneSearchRequest request, CancellationToken cancellationToken);
}
