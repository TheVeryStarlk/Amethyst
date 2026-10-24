using System.Diagnostics.CodeAnalysis;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Worlds;
using Amethyst.Eventing;
using Amethyst.Worlds;
using Microsoft.Extensions.DependencyInjection;

namespace Amethyst.Hosting;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAmethyst<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(this IServiceCollection services) where T : class, ISubscriber
    {
        services.AddSubscriber<T>();

        services.AddSingleton<EventDispatcher>();

        services.AddSingleton<IWorldFactory, WorldFactory>();

        services.AddTransient<Server>();

        services.AddHostedService<AmethystService>();
        services.AddHostedService<KeepAliveService>();

        return services;
    }

    /// <summary>
    /// Registers an additional <see cref="ISubscriber"/>, subscribers run in the order they were registered.
    /// </summary>
    public static IServiceCollection AddSubscriber<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(this IServiceCollection services) where T : class, ISubscriber
    {
        services.AddTransient<ISubscriber, T>();
        return services;
    }
}
