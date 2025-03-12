using System.Diagnostics.CodeAnalysis;
using Amethyst.Eventing;
using Microsoft.Extensions.DependencyInjection;

namespace Amethyst.Hosting;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAmethyst<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(this IServiceCollection services) where T : class, ISubscriber
    {
        services.AddTransient<T>();

        services.AddHostedService<AmethystService>();

        return services;
    }
}