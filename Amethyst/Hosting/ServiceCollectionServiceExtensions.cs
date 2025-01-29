using Amethyst.Hosting.Subscriber;
using Microsoft.Extensions.DependencyInjection;

namespace Amethyst.Hosting;

public static class ServiceCollectionServiceExtensions
{
    public static IServiceCollection AddAmethyst<T>(this IServiceCollection services, Action<AmethystOptions> configure) where T : ISubscriber
    {
        var options = new AmethystOptions();
        configure(options);

        return services;
    }
}

public sealed class AmethystOptions
{
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);
}