using System.Diagnostics.CodeAnalysis;
using Amethyst.Components.Eventing;

namespace Amethyst.Components.Hosting;

public interface IAmethystBuilder
{
    public IAmethystBuilder AddSubscriber<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>() where T : class, ISubscriber;
}