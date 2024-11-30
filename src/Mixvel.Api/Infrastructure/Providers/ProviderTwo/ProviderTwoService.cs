using Mixvel.Api.Core.Interfaces;

namespace Mixvel.Api.Infrastructure.Providers.ProviderTwo;

internal sealed class ProviderTwoService(HttpClient httpClient, ILogger<ProviderTwoService> logger) : IProviderTwoService
{
    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
    {
        try
        {
            var httpResponseMessage = await httpClient.GetAsync("/api/v1/ping", cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while executing the request");
            return false;
        }
    }

    public async Task<ProviderTwoSearchResponse?> SearchAsync(ProviderTwoSearchRequest request, CancellationToken cancellationToken)
    {
        try
        {
            var httpResponseMessage = await httpClient.PostAsJsonAsync("/api/v1/search", request, cancellationToken);
            httpResponseMessage.EnsureSuccessStatusCode();

            return await httpResponseMessage.Content
                .ReadFromJsonAsync<ProviderTwoSearchResponse>(cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while executing the request");
            return null;
        }
    }
}
