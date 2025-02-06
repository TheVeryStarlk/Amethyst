using System.Diagnostics.CodeAnalysis;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Amethyst.Hosting;

internal sealed class AmethystBuilder(IServiceCollection services) : IAmethystBuilder
{
    public IAmethystBuilder AddSubscriber<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>() where T : class, ISubscriber
    {
        services.AddTransient<T>();
        return this;
    }
}