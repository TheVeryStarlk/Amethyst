using System.Diagnostics.CodeAnalysis;
using Amethyst.Components.Eventing;
using Microsoft.Extensions.DependencyInjection;

namespace Amethyst.Hosting;

public static class ServiceCollectionServiceExtensions
{
    public static IServiceCollection AddAmethyst<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>(this IServiceCollection services, Action<AmethystOptions> configure) where T : Subscriber
    {
        var options = new AmethystOptions();
        configure(options);

        services.AddTransient<Subscriber, T>();

        return services;
    }
}

public sealed class AmethystOptions
{
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
}