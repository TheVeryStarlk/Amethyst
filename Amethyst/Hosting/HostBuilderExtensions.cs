﻿using Amethyst.Plugins;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Amethyst.Hosting;

public static class HostBuilderExtensions
{
    public static IHostBuilder ConfigureMinecraftServer(
        this IHostBuilder builder,
        Action<HostBuilderContext, ServerOptions> configure)
    {
        builder.ConfigureServices((context, services) =>
        {
            var options = new ServerOptions();
            configure.Invoke(context, options);

            services.AddTransient<CommandService>();
            services.AddTransient<EventService>();
            services.AddSingleton<PluginService>();
            services.AddTransient<IConnectionListenerFactory, SocketTransportFactory>();

            services.AddTransient(provider => new Server(
                options,
                provider.GetRequiredService<IConnectionListenerFactory>(),
                provider.GetRequiredService<ILoggerFactory>(),
                provider.GetRequiredService<PluginService>(),
                provider.GetRequiredService<IHostApplicationLifetime>().ApplicationStopping));

            services.AddHostedService<ServerService>();
        });

        return builder;
    }
}