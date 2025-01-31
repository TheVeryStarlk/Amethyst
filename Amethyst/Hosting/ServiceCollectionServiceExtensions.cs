using System.Diagnostics.CodeAnalysis;
using Amethyst.Components.Eventing;
using Amethyst.Eventing;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.DependencyInjection;

namespace Amethyst.Hosting;

public static class ServiceCollectionServiceExtensions
{
    public static IServiceCollection AddAmethyst<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(this IServiceCollection services) where T : class, ISubscriber
    {
        services.AddTransient<ISubscriber, T>();
        services.AddTransient<IConnectionListenerFactory, SocketTransportFactory>();
        services.AddTransient<EventDispatcher>();
        services.AddTransient<Server>();

        services.AddHostedService<AmethystService>();

        return services;
    }
}