using System.Diagnostics.CodeAnalysis;
using Amethyst.Components.Eventing;
using Amethyst.Components.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Amethyst.Hosting;

internal sealed class AmethystBuilder(IServiceCollection services) : IAmethystBuilder
{
    public IAmethystBuilder AddSubscriber<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>() where T : class, ISubscriber
    {
        services.AddSingleton<ISubscriber, T>();
        return this;
    }
}