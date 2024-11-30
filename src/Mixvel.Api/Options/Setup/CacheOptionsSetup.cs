using Microsoft.Extensions.Options;

namespace Mixvel.Api.Options.Setup;

internal sealed class CacheOptionsSetup(IConfiguration configuration) : IConfigureOptions<CacheOptions>
{
    private const string _sectionName = "CacheOptions";
    public void Configure(CacheOptions options)
    {
        configuration.GetSection(_sectionName).Bind(options);
    }
}
