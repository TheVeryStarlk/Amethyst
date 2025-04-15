using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Networking.Packets.Play;
using Amethyst.Abstractions.Worlds;
using Amethyst.Eventing;

namespace Amethyst.Networking.Packets.Play;

internal sealed class PlacementPacket(Position position, BlockFace face, Item item) : IIngoingPacket<PlacementPacket>, IProcessor
{
    public static int Identifier => 8;

    public static PlacementPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new PlacementPacket(reader.ReadPosition(), (BlockFace) reader.ReadByte(), reader.ReadItem());
    }

    public void Process(Client client, EventDispatcher eventDispatcher)
    {
        if (item.Type is -1 || face is BlockFace.Self)
        {
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

        client.Player!.World[(int) where.X, (int) where.Y, (int) where.Z] = block;

        var packet = new BlockPacket(where, block);

        foreach (var pair in client.Player.World.Players.Where(pair => pair.Value != client.Player))
        {
            pair.Value.Client.Write(packet);
        }
    }
}