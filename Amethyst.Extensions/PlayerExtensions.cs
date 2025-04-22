using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Extensions;

public static class PlayerExtensions
{
    public static void Send(this IPlayer player, Message message, MessagePosition position = MessagePosition.Box)
    {
        player.Client.Write(new MessagePacket(message, position));
    }

    public static void Disconnect(this IPlayer player, Message message)
    {
        player.Client.Write(new DisconnectPacket(message));
        player.Client.Stop();
    }
}