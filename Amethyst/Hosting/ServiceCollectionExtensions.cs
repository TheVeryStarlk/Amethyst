using System.Diagnostics.CodeAnalysis;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Worlds;
using Amethyst.Eventing;
using Amethyst.Worlds;
using Microsoft.Extensions.DependencyInjection;

namespace Amethyst.Hosting;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddAmethyst<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(Action<AmethystOptions> configure) where T : class, ISubscriber
        {
            services.Configure(configure);

            services.AddSubscriber<T>();

            services.AddSingleton<Dispatcher>();
            services.AddSingleton<ClientFactory>();

            services.AddSingleton<IWorldFactory, WorldFactory>();

            services.AddHostedService<AmethystService>();
            services.AddHostedService<KeepAliveService>();

            return services;
        }

        /// <summary>
        /// Registers an additional <see cref="ISubscriber"/>, subscribers run in the order they were registered.
        /// </summary>
        public IServiceCollection AddSubscriber<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>() where T : class, ISubscriber
        {
            services.AddTransient<ISubscriber, T>();
            return services;
        }
    }
}