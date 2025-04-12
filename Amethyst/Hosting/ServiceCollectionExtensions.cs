using System.Diagnostics.CodeAnalysis;
using Amethyst.Abstractions.Worlds;
using Amethyst.Eventing;
using Amethyst.Worlds;
using Microsoft.Extensions.DependencyInjection;

namespace Amethyst.Hosting;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAmethyst<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(this IServiceCollection services) where T : class, ISubscriber
    {
        services.AddTransient<ISubscriber, AmethystSubscriber>();
        services.AddTransient<ISubscriber, T>();

        services.AddSingleton<EventDispatcher>();
        services.AddSingleton<IWorldFactory, WorldFactory>();

        services.AddTransient<Server>();

        services.AddHostedService<AmethystService>();

        return services;
    }
}