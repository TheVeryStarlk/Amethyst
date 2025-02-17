using Amethyst.Components.Entities;
using Amethyst.Entities;
using Amethyst.Eventing;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.DependencyInjection;

namespace Amethyst.Hosting;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAmethyst(this IServiceCollection services, Action<AmethystOptions> configure)
    {
        configure(new AmethystOptions(services).AddSubscriber<AmethystSubscriber>());

        services.AddSingleton<EventDispatcher>();
        services.AddSingleton<IPlayerStore, PlayerStore>();

        services.AddTransient<IConnectionListenerFactory, SocketTransportFactory>();
        services.AddTransient<Server>();

        services.AddHostedService<AmethystService>();

        return services;
    }
}