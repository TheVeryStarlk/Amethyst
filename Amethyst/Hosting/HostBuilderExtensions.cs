﻿using Amethyst.Api;
using Amethyst.Plugins;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Amethyst.Hosting;

public static class HostBuilderExtensions
{
    public static IHostBuilder ConfigureServer(
        this IHostBuilder builder,
        Action<HostBuilderContext, ServerOptions> configure)
    {
        builder.ConfigureServices((context, services) =>
        {
            var options = new ServerOptions();
            configure.Invoke(context, options);
            services.AddTransient(_ => options);

            services.AddSingleton<PluginService>();
            services.AddTransient<PluginRegistry>();
            services.AddSingleton<EventDispatcher>();
            services.AddTransient<CommandExecutor>();

            services.AddTransient<IConnectionListenerFactory, SocketTransportFactory>();
            services.AddSingleton<Server>();

            services.AddHostedService<WorldService>();
            services.AddHostedService<ServerService>();
        });

        return builder;
    }
}