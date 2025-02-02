namespace Amethyst.Components.Eventing.Sources.Client;

public sealed class Outdated(int version) : Event<IClient>
{
    public int Version { get; init; } = version;
}