using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Abstractions.Eventing.Player;

public sealed class Configuration(string locale, byte viewDistance) : IEvent<IPlayer>
{
    public string Locale { get; set; } = locale;

    public byte ViewDistance { get; set; } = viewDistance;
}