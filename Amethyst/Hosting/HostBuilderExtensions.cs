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
        Action<HostBuilderContext, MinecraftServerConfiguration> configure)
    {
        builder.ConfigureServices((context, services) =>
        {
            var configuration = new MinecraftServerConfiguration();
            configure.Invoke(context, configuration);

            services.AddTransient<IConnectionListenerFactory, SocketTransportFactory>();

            services.AddTransient(provider => new MinecraftServer(
                configuration.ListeningEndPoint,
                provider.GetRequiredService<IConnectionListenerFactory>(),
                provider.GetRequiredService<ILoggerFactory>()));

            services.AddHostedService<MinecraftServerService>();
        });

        return builder;
    }
}