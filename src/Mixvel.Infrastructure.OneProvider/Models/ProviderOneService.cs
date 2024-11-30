namespace Mixvel.Infrastructure.OneProvider.Models;

//internal sealed class ProviderOneService(HttpClient httpClient, ILogger<ProviderOneService> logger) : IProviderOneService
//{
//    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken)
//    {
//        try
//        {
//            var httpResponseMessage = await httpClient.GetAsync("/api/v1/ping", cancellationToken);
//            httpResponseMessage.EnsureSuccessStatusCode();

//            return true;
//        }
//        catch (Exception ex)
//        {
//            logger.LogError(ex, "An error occurred while executing the request");
//            return false;
//        }
//    }

//    public async Task<ProviderOneSearchResponse?> SearchAsync(ProviderOneSearchRequest request, CancellationToken cancellationToken)
//    {
//        try
//        {
//            var httpResponseMessage = await httpClient.PostAsJsonAsync("/api/v1/search", request, cancellationToken);
//            httpResponseMessage.EnsureSuccessStatusCode();

//            return await httpResponseMessage.Content
//                .ReadFromJsonAsync<ProviderOneSearchResponse>(cancellationToken: cancellationToken);
//        }
//        catch (Exception ex)
//        {
//            logger.LogError(ex, "An error occurred while executing the request");
//            return null;
//        }
//    }
//}
