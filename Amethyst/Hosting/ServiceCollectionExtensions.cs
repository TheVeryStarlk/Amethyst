using System.Diagnostics.CodeAnalysis;
using Amethyst.Abstractions.Worlds;
using Amethyst.Entities;
using Amethyst.Eventing;
using Amethyst.Hosting.Subscribers;
using Amethyst.Worlds;
using Microsoft.Extensions.DependencyInjection;

namespace Amethyst.Hosting;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAmethyst<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(this IServiceCollection services) where T : class, ISubscriber
    {
        services.AddSingleton<ISubscriber, T>();

        services.AddSingleton<ISubscriber, PlayerSubscriber>();
        services.AddSingleton<ISubscriber, WorldSubscriber>();
        services.AddSingleton<ISubscriber, MessageSubscriber>();

        services.AddSingleton<EventDispatcher>();
        services.AddSingleton<WorldStore>();
        services.AddSingleton<PlayerStore>();

        services.AddTransient<Func<string, IGenerator, IWorld>>(provider => (name, generator) => new World(name, generator, provider.GetRequiredService<PlayerStore>()));

        services.AddTransient<Server>();

        services.AddHostedService<AmethystService>();

        return services;
    }
}