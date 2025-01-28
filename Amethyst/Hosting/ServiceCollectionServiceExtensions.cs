using Microsoft.Extensions.DependencyInjection;

namespace Amethyst.Hosting;

public static class ServiceCollectionServiceExtensions
{
    public static IServiceCollection AddAmethyst(this IServiceCollection services)
    {
        return services;
    }
}