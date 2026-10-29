using System.Diagnostics.CodeAnalysis;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Worlds;
using Amethyst.Eventing;
using Amethyst.Networking.Ingoing;
using Amethyst.Networking.Ingoing.Processors;
using Amethyst.Worlds;
using Microsoft.Extensions.DependencyInjection;

namespace Amethyst.Hosting;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddAmethyst()
        {
            services.AddOptions<AmethystOptions>();


            services.AddSingleton<HandshakeProcessor>();
            services.AddSingleton<StatusProcessor>();
            services.AddSingleton<LoginProcessor>();
            services.AddSingleton<MovementProcessor>();
            services.AddSingleton<ChatProcessor>();
            services.AddSingleton<BlockProcessor>();
            services.AddSingleton<InventoryProcessor>();
            services.AddSingleton<PlayerProcessor>();
            services.AddSingleton<Router>();
            services.AddSingleton<ClientFactory>();

            services.AddSingleton<Dispatcher>();
            services.AddSingleton<IWorldFactory, WorldFactory>();

            services.AddHostedService<AmethystService>();
            services.AddHostedService<KeepAliveService>();

            return services;
        }

        public IServiceCollection AddAmethyst(Action<AmethystOptions> configure)
        {
            services.Configure(configure);
            return services.AddAmethyst();
        }

        /// <summary>
        /// Registers a <see cref="ISubscriber"/>.
        /// </summary>
        /// <remarks>
        /// <see cref="ISubscriber"/>s run in the order they were registered.
        /// </remarks>
        public IServiceCollection AddSubscriber<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>() where T : class, ISubscriber
        {
            services.AddTransient<ISubscriber, T>();
            return services;
        }
    }
}