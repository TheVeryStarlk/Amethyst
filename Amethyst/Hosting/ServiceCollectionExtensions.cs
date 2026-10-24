using System.Diagnostics.CodeAnalysis;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Worlds;
using Amethyst.Entities.Players;
using Amethyst.Eventing;
using Amethyst.Worlds;
using Microsoft.Extensions.DependencyInjection;

namespace Amethyst.Hosting;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAmethyst<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(this IServiceCollection services) where T : class, ISubscriber
    {
        services.AddTransient<ISubscriber, AmethystSubscriber>();
        services.AddSubscriber<T>();

        services.AddSingleton<EventDispatcher>();
        services.AddSingleton<PlayerRepository>();

        services.AddSingleton<IWorldFactory, WorldFactory>();
        services.AddSingleton<IInventoryViewer, InventoryViewer>();


        services.AddTransient<Server>();

        services.AddHostedService<AmethystService>();

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