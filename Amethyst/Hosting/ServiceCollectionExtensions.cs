using Amethyst.Abstractions.Worlds;
using Amethyst.Eventing;
using Amethyst.Hosting.Subscribers;
using Amethyst.Worlds;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.DependencyInjection;

namespace Amethyst.Hosting;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAmethyst(this IServiceCollection services, Action<AmethystOptions> configure)
    {
        var options = new AmethystOptions(services)
            .AddSubscriber<PlayerSubscriber>()
            .AddSubscriber<WorldSubscriber>();

        configure(options);

        services.AddSingleton<EventDispatcher>();
        services.AddSingleton<WorldStore>();
        services.AddSingleton<IWorldManager, WorldManager>();

        services.AddTransient<IConnectionListenerFactory, SocketTransportFactory>();
        services.AddTransient<Server>();

        services.AddHostedService<AmethystService>();

        return services;
    }
}