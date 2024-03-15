using Amethyst.Api;
using Amethyst.Api.Plugins;
using Amethyst.Plugins;
using Amethyst.Services;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Amethyst.Hosting;

public static class HostBuilderExtensions
{
    public static IHostBuilder ConfigureServer(
        this IHostBuilder builder,
        Action<HostBuilderContext, ServerConfiguration> configure)
    {
        builder.ConfigureServices((context, services) =>
        {
            var configuration = new ServerConfiguration();
            configure.Invoke(context, configuration);
            services.AddTransient(_ => configuration);

            services.AddTransient<IPluginRegistry, PluginRegistry>();

            services.AddSingleton<CommandService>();
            services.AddSingleton<EventService>();
            services.AddSingleton<PluginService>();

            services.AddTransient<IConnectionListenerFactory, SocketTransportFactory>();
            services.AddSingleton<Server>();
            services.AddHostedService<ServerService>();
        });

        return builder;
    }
}