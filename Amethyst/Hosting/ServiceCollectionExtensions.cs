using Amethyst.Abstractions.Worlds;
using Amethyst.Entities;
using Amethyst.Eventing;
using Amethyst.Hosting.Subscribers;
using Amethyst.Worlds;
using Microsoft.Extensions.DependencyInjection;

namespace Amethyst.Hosting;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAmethyst(this IServiceCollection services, Action<AmethystOptions> configure)
    {
        var options = new AmethystOptions(services)
            .AddSubscriber<PlayerSubscriber>()
            .AddSubscriber<WorldSubscriber>();

        configure(options);

        // This needs information from previous subscribers. So it should run the last.
        // Perhaps all subscribers could run last, too.
        options.AddSubscriber<MessageSubscriber>();

        services.AddSingleton<EventDispatcher>();
        services.AddSingleton<WorldStore>();
        services.AddSingleton<PlayerStore>();

        services.AddTransient<Func<string, IGenerator, IWorld>>(provider => (name, generator) => new World(name, generator, provider.GetRequiredService<PlayerStore>()));

        services.AddTransient<Server>();

        services.AddHostedService<AmethystService>();

        return services;
    }
}