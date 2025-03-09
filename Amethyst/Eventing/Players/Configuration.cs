using Amethyst.Abstractions.Entities;

namespace Amethyst.Eventing.Players;

public sealed class Configuration(string locale, byte viewDistance) : Event<IPlayer>
{
    public string Locale => locale;

    public byte ViewDistance => viewDistance;
}