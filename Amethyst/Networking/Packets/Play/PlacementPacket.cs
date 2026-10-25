using System.Diagnostics;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Abstractions.Worlds;
using Amethyst.Eventing;
using Amethyst.Abstractions.Eventing.Player;
using Raspite;
using Raspite.Tags;

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

        var amount = reader.ReadByte();
        var durability = reader.ReadShort();

        // The cursor position that follows the NBT is not read.
        var remaining = span[reader.Position..];

        // Items without NBT only have a single end tag.
        if (remaining[0] is Tag.End)
        {
            return new PlacementPacket(position, face, new Item(type, amount, durability));
        }

        if (!TagSerializer.TryParse<CompoundTag>(remaining, out var tag))
        {
            throw new InvalidOperationException("Invalid NBT.");
        }

        return new PlacementPacket(position, face, new Item(type, amount, durability, tag));
    }

    // This needs more work.
    public void Process(Client client, Dispatcher dispatcher)
    {
        Debug.Assert(client.Player is not null);

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
            _ => throw new ArgumentOutOfRangeException(nameof(face), face, "Invalid block face.")
        };

        var block = new Block(item.Type, item.Durability);
        var placed = dispatcher.Dispatch(client.Player, new Placed(where, block));

        if (placed.IsCancelled)
        {
            // The client has already placed the block on its side.
            client.Write(new BlockPacket(where, client.Player.World[where]));
        }
    }
}