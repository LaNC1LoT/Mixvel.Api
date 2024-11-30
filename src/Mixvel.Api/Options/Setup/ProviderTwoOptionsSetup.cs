using Microsoft.Extensions.Options;

namespace Mixvel.Api.Options.Setup;

internal sealed class ProviderTwoOptionsSetup(IConfiguration configuration) : IConfigureOptions<ProviderTwoOptions>
{
    private const string _sectionName = "ProviderTwoOptions";
    public void Configure(ProviderTwoOptions options)
    {
        configuration.GetSection(_sectionName).Bind(options);
    }
}
