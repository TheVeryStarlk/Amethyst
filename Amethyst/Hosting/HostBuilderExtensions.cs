using Amethyst.Api;
using Amethyst.Plugins;
using Amethyst.Services;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

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

            services.AddTransient<CommandService>();
            services.AddTransient<EventService>();
            services.AddSingleton<PluginService>();
            services.AddTransient<IConnectionListenerFactory, SocketTransportFactory>();

            services.AddTransient(provider => new Server(
                configuration,
                provider.GetRequiredService<IConnectionListenerFactory>(),
                provider.GetRequiredService<ILoggerFactory>(),
                provider.GetRequiredService<PluginService>()));

            services.AddHostedService<ServerService>();
        });

        return builder;
    }
}