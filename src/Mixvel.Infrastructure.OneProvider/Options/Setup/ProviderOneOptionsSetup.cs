using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Mixvel.Infrastructure.OneProvider.Options.Setup;

internal sealed class ProviderOneOptionsSetup(IConfiguration configuration) : IConfigureOptions<ProviderOneOptions>
{
    private const string _sectionName = "ProviderOneOptions";
    public void Configure(ProviderOneOptions options)
    {
        configuration.GetSection(_sectionName).Bind(options);
    }
}
