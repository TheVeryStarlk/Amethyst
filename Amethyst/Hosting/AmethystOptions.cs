using System.Diagnostics.CodeAnalysis;
using Amethyst.Abstractions.Eventing;
using Microsoft.Extensions.DependencyInjection;

namespace Amethyst.Hosting;

public sealed class AmethystOptions
{
    private readonly IServiceCollection services;

    internal AmethystOptions(IServiceCollection services)
    {
        this.services = services;
    }

    public AmethystOptions AddSubscriber<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>() where T : class, ISubscriber
    {
        services.AddSingleton<ISubscriber, T>();
        return this;
    }
}