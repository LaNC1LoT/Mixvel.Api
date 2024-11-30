using Mixvel.Api.Infrastructure.Providers.ProviderTwo;

namespace Mixvel.Api.Core.Interfaces;

public interface IProviderTwoService
{
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken);
    Task<ProviderTwoSearchResponse?> SearchAsync(ProviderTwoSearchRequest request, CancellationToken cancellationToken);
}
