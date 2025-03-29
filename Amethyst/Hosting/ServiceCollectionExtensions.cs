using System.Diagnostics.CodeAnalysis;
using Amethyst.Eventing;
using Amethyst.Hosting.Subscribers;
using Microsoft.Extensions.DependencyInjection;

namespace Amethyst.Hosting;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAmethyst<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(this IServiceCollection services) where T : class, ISubscriber
    {
        services.AddTransient<ISubscriber, PlayerSubscriber>();
        services.AddTransient<ISubscriber, T>();

        services.AddSingleton<EventDispatcher>();

        services.AddHostedService<AmethystService>();

        return services;
    }
}