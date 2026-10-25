using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Extensions;

public static class PlayerExtensions
{
    extension(IPlayer player)
    {
        public void Send(Message message, MessagePosition position = MessagePosition.Box)
        {
            player.Client.Write(new MessagePacket(message, position));
        }

        public void Disconnect(Message message)
        {
            player.Client.Write(new DisconnectPacket(message));
            player.Client.Stop();
        }
    }
}