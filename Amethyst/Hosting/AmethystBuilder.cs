using System.Diagnostics.CodeAnalysis;
using Amethyst.Components.Eventing;
using Amethyst.Components.Hosting;
using Amethyst.Eventing;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.DependencyInjection;

namespace Amethyst.Hosting;

internal sealed class AmethystBuilder(IServiceCollection services) : IAmethystBuilder
{
    public IAmethystBuilder AddSubscriber<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>() where T : class, ISubscriber
    {
        services.AddSingleton<ISubscriber, T>();
        services.AddSingleton<EventDispatcher>();

        services.AddTransient<IConnectionListenerFactory, SocketTransportFactory>();
        services.AddTransient<Server>();

        services.AddHostedService<AmethystService>();

        return this;
    }
}