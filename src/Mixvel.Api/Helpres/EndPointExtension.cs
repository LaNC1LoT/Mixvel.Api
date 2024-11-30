using Microsoft.Extensions.DependencyInjection.Extensions;
using Mixvel.Api.EndPoints;
using System.Reflection;

namespace Mixvel.Api.DependencyInjection;

internal static class EndPointExtension
{
    public static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        var end = assembly
            .DefinedTypes
            .Where(type => type is { IsAbstract: false, IsInterface: false } &&
                           type.IsAssignableTo(typeof(IEndPoint)))
            .Select(type => ServiceDescriptor.Transient(typeof(IEndPoint), type))
            .ToArray();

        services.TryAddEnumerable(end);

        return services;
    }

    public static IApplicationBuilder MapEndpoints(this WebApplication app)
    {
        var endPoints = app.Services.GetRequiredService<IEnumerable<IEndPoint>>();
        foreach (var endPoint in endPoints)
        {
            endPoint.MapEndPoint(app);
        }
        return app;
    }
}
