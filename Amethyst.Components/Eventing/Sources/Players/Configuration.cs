using Amethyst.Components.Entities;

namespace Amethyst.Components.Eventing.Sources.Players;

public sealed class Configuration(string locale, byte viewDistance) : Event<IPlayer>
{
    public string Locale => locale;

    public byte ViewDistance => viewDistance;
}