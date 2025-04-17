using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Abstractions.Packets.Login;

internal sealed class SuccessPacket(IPlayer player) : IOutgoingPacket
{
    public int Identifier => 2;

    public IPlayer Player { get; } = player;
}