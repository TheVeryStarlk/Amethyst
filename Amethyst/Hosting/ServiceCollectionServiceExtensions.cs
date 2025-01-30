using System.Diagnostics.CodeAnalysis;
using System.Net;
using Amethyst.Components.Eventing;
using Amethyst.Eventing;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.DependencyInjection;

namespace Amethyst.Hosting;

public static class ServiceCollectionServiceExtensions
{
    public static IServiceCollection AddAmethyst<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(this IServiceCollection services, Action<AmethystOptions> configure) where T : class, ISubscriber
    {
        var options = new AmethystOptions();
        configure(options);

        services.AddTransient<ISubscriber, T>();
        services.AddTransient<IConnectionListenerFactory, SocketTransportFactory>();
        services.AddTransient<AmethystOptions>(_ => options);
        services.AddTransient<EventDispatcher>();
        services.AddTransient<Server>();

        return services;
    }
}

public sealed class AmethystOptions
{
    public IPEndPoint EndPoint { get; set; } = new(IPAddress.Any, 25565);

    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
}