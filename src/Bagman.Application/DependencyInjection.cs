using System.Reflection;
using Bagman.Application.Common;
using Microsoft.Extensions.DependencyInjection;

namespace Bagman.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Register FeatureDispatcher
        services.AddScoped<FeatureDispatcher>();

        // Auto-register all handlers from assembly
        var assembly = Assembly.GetExecutingAssembly();

        var handlerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces()
                .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IFeatureHandler<,>)))
            .ToList();

        foreach (var handlerType in handlerTypes)
        {
            var handlerInterfaces = handlerType.GetInterfaces()
                .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IFeatureHandler<,>));

            foreach (var handlerInterface in handlerInterfaces)
                services.AddScoped(handlerInterface, handlerType);
        }

        return services;
    }
}
