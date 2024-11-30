using Microsoft.Extensions.Options;

namespace Mixvel.Api.Options.Setup;

internal sealed class ProviderOneOptionsSetup(IConfiguration configuration) : IConfigureOptions<ProviderOneOptions>
{
    private const string _sectionName = "ProviderOneOptions";
    public void Configure(ProviderOneOptions options)
    {
        configuration.GetSection(_sectionName).Bind(options);
    }
}
