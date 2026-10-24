using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Abstractions.Worlds;
using Amethyst.Eventing;
using Amethyst.Abstractions.Eventing.Player;

namespace Amethyst.Networking.Packets.Play;

internal sealed class PlacementPacket(Position position, BlockFace face, Item item) : IIngoingPacket<PlacementPacket>, IProcessor
{
    public static int Identifier => 8;

    public static PlacementPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);

        var position = reader.ReadPosition();
        var face = (BlockFace) reader.ReadByte();
        var type = reader.ReadShort();

        if (type is -1)
        {
            return new PlacementPacket(position, face, new Item(-1, 0, 0));
        }

        var item = new Item(type, reader.ReadByte(), reader.ReadShort());

        if (reader.ReadBoolean())
        {
            throw new InvalidOperationException("Has NBT.");
        }

        return new PlacementPacket(position, face, item);
    }

    // This needs more work.
    public void Process(Client client, EventDispatcher eventDispatcher)
    {
        if (item.Type is -1 or >= 256 || face is BlockFace.Self && position.X is -1 && position.Y is -1 && position.Z is -1)
        {
            eventDispatcher.Dispatch(client.Player!, new Use(position, item));
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
            _ => throw new ArgumentOutOfRangeException(nameof(face), face, "Invalid block face.")
        };

        var block = new Block(item.Type, item.Durability);
        var placed = eventDispatcher.Dispatch(client.Player!, new Placed(where, block));

        if (placed.IsCancelled)
        {
            // The client has already placed the block on its side.
            client.Write(new BlockPacket(where, client.Player!.World[where]));
            return;
        }

        client.Player!.World[where] = block;

        var packet = new BlockPacket(where, block);

        foreach (var pair in client.Player.World.Players.Where(pair => pair.Value != client.Player))
        {
            pair.Value.Client.Write(packet);
        }
    }
}
