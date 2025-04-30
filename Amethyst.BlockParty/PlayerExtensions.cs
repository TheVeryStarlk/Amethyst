using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.BlockParty;

internal static class PlayerExtensions
{
    public static void Send(this IPlayer player, Message message, MessagePosition position = MessagePosition.Box)
    {
        player.Client.Write(new MessagePacket(message, position));
    }
}