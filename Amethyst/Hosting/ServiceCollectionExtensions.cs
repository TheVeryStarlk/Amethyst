using Amethyst.Components.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Amethyst.Hosting;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAmethyst(this IServiceCollection services, Action<IAmethystBuilder> configure)
    {
        configure(new AmethystBuilder(services));
        return services;
    }
}