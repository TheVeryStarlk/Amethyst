using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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

            services.AddTransient(provider =>
            {
                var loggerFactory = provider.GetRequiredService<ILoggerFactory>();

                var listenerFactory = new SocketTransportFactory(
                    Options.Create(new SocketTransportOptions()),
                    loggerFactory);

                return new MinecraftServer(
                    configuration.ListeningEndPoint,
                    listenerFactory,
                    loggerFactory);
            });

            services.AddHostedService<MinecraftServerService>();
        });

        return builder;
    }
}