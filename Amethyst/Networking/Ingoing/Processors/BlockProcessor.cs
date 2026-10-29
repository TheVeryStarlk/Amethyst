using System.Diagnostics;
using Amethyst.Abstractions.Eventing.Player;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Abstractions.Worlds;
using Amethyst.Eventing;
using Amethyst.Networking.Ingoing.Packets.Play;

namespace Amethyst.Networking.Ingoing.Processors;

internal sealed class BlockProcessor(Dispatcher dispatcher) : IProcessor<DiggingPacket>, IProcessor<PlacementPacket>
{
    public void Process(Client client, DiggingPacket packet)
    {
        Debug.Assert(client.Player is not null);

        if (packet.Digging is not (Digging.Started or Digging.Cancelled or Digging.Finished))
        {
            return;
        }

        var dig = dispatcher.Dispatch(client.Player, new Dig(packet.Digging, packet.Position, packet.Face));

        if (dig.IsCancelled)
        {
            client.Write(new BlockPacket(packet.Position, client.Player.World[packet.Position]));
        }
    }

    // This needs more work.
    public void Process(Client client, PlacementPacket packet)
    {
        Debug.Assert(client.Player is not null);

        var position = packet.Position;
        var face = packet.Face;
        var item = packet.Item;

        if (item.Type is -1 or >= 256 || face is BlockFace.Self && position.X is -1 && position.Y is -1 && position.Z is -1)
        {
            dispatcher.Dispatch(client.Player, new Use(position, item));
            return;
        }

        var where = face switch
        {
            BlockFace.NegativeY => position with { Y = position.Y - 1 },
            BlockFace.PositiveY => position with { Y = position.Y + 1 },
            BlockFace.NegativeZ => position with { Z = position.Z - 1 },
            BlockFace.PositiveZ => position with { Z = position.Z + 1 },
            BlockFace.NegativeX => position with { X = position.X - 1 },
            BlockFace.PositiveX => position with { X = position.X + 1 },
            _ => throw new ArgumentOutOfRangeException(nameof(packet), face, "Invalid block face.")
        };

        var block = new Block(item.Type, item.Durability);
        var placed = dispatcher.Dispatch(client.Player, new Placed(where, block));

        if (placed.IsCancelled)
        {
            client.Write(new BlockPacket(where, client.Player.World[where]));
        }
    }
}