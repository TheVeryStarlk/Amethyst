using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Abstractions.Worlds;
using Amethyst.Eventing;
using Amethyst.Abstractions.Eventing.Player;

namespace Amethyst.Networking.Packets.Play;

internal sealed class DiggingPacket(Digging digging, Position position, BlockFace face) : IIngoingPacket<DiggingPacket>, IProcessor
{
    public static int Identifier => 7;

    public static DiggingPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new DiggingPacket((Digging) reader.ReadByte(), reader.ReadPosition(), (BlockFace) reader.ReadByte());
    }

    public void Process(Client client, EventDispatcher eventDispatcher)
    {
        var dig = eventDispatcher.Dispatch(client.Player!, new Dig(digging, position, face));

        if (dig.IsCancelled)
        {
            // The client has already removed the block on its side.
            client.Write(new BlockPacket(position, client.Player!.World[position]));
            return;
        }

        var packet = new BlockPacket(position, Blocks.Air);

        foreach (var pair in client.Player!.World.Players.Where(pair => pair.Value != client.Player))
        {
            pair.Value.Client.Write(packet);
        }

        client.Player.World[position] = Blocks.Air;
    }
}