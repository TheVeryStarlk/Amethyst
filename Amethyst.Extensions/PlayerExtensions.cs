using System.Runtime.CompilerServices;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Extensions;

public static class PlayerExtensions
{
    // Entries are dropped once the player is collected, no need to clean up when players leave.
    private static readonly ConditionalWeakTable<IPlayer, Inventory> inventories = new();

    extension(IPlayer player)
    {
        /// <remarks>Requires <see cref="Subscribers.InventorySubscriber"/> to track changes made by the player.</remarks>
        public Inventory Inventory => inventories.GetValue(player, static player => new Inventory(player));
    }

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
