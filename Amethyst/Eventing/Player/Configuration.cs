using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Eventing.Player;

public sealed class Configuration(string locale, byte viewDistance) : IEvent<IPlayer>
{
    public string Locale => locale;

    public byte ViewDistance => viewDistance;
}