using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions.Eventing.Sources.Players;

public sealed class Configuration(string locale, byte viewDistance) : Event<IPlayer>
{
    public string Locale => locale;

    public byte ViewDistance => viewDistance;
}