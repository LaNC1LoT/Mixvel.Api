using Microsoft.Extensions.Options;
using Mixvel.Api.Core.Interfaces;
using Mixvel.Api.Infrastructure;
using Mixvel.Api.Infrastructure.Aggregators;
using Mixvel.Api.Infrastructure.Providers.ProviderOne;
using Mixvel.Api.Infrastructure.Providers.ProviderTwo;
using Mixvel.Api.Options;
using System.Reflection;

namespace Mixvel.Api.DependencyInjection;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();

        services.AddMemoryCache();
        services.AddSingleton<ICacheService, MemoryCacheService>();

        services.ConfigureOptionsFromAssembly(assembly);

        services.AddHttpClient<IProviderOneService, ProviderOneService>()
            .ConfigureHttpClient((provider, client) =>
            {
                var options = provider.GetRequiredService<IOptions<ProviderOneOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseAddress);
                client.Timeout = TimeSpan.FromMilliseconds(options.TimeoutInMls);
            });

        services.AddHttpClient<IProviderTwoService, ProviderTwoService>()
            .ConfigureHttpClient((provider, client) =>
            {
                var options = provider.GetRequiredService<IOptions<ProviderTwoOptions>>().Value;
                client.BaseAddress = new Uri(options.BaseAddress);
                client.Timeout = TimeSpan.FromMilliseconds(options.TimeoutInMls);
            });

        services.AddImplementations<IProviderAdapter>(assembly, ServiceLifetime.Transient);

        services.AddSingleton<ISearchService, AggregatedSearchService>();

        return services;
    }

    public static IServiceCollection ConfigureOptionsFromAssembly(this IServiceCollection services, Assembly assembly)
    {
        var configureOptionsType = typeof(IConfigureOptions<>);

        var implementations = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract)
            .Where(t => t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == configureOptionsType));

        foreach (var implementation in implementations)
        {
            services.ConfigureOptions(implementation);
        }

        return services;
    }

    public static IServiceCollection AddImplementations<TInterface>(
        this IServiceCollection services,
        Assembly assembly,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
    {
        var interfaceType = typeof(TInterface);
        var implementations = assembly.GetTypes()
            .Where(t => interfaceType.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);

        foreach (var implementation in implementations)
        {
            services.Add(new ServiceDescriptor(interfaceType, implementation, lifetime));
        }

        return services;
    }
}
