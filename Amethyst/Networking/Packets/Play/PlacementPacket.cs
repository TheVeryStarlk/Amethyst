using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Abstractions.Worlds;
using Amethyst.Eventing;
using Amethyst.Eventing.Player;

namespace Amethyst.Networking.Packets.Play;

internal sealed class PlacementPacket(Position position, BlockFace face, Item item) : IIngoingPacket<PlacementPacket>, IProcessor
{
    public static int Identifier => 8;

    public static PlacementPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new PlacementPacket(reader.ReadPosition(), (BlockFace) reader.ReadByte(), reader.ReadItem());
    }

    // This needs more work.
    public void Process(Client client, EventDispatcher eventDispatcher)
    {
        if (item.Type is -1 or >= 256 || face is BlockFace.Self && position.X is -1 && position.Y is -1 && position.Z is -1)
        {
            eventDispatcher.Dispatch(client.Player, new Use(position, item));
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

        client.Player!.World[where] = block;

        var packet = new BlockPacket(where, block);

        foreach (var pair in client.Player.World.Players.Where(pair => pair.Value != client.Player))
        {
            pair.Value.Client.Write(packet);
        }

        eventDispatcher.Dispatch(client.Player, new Placed(where, block));
    }
}