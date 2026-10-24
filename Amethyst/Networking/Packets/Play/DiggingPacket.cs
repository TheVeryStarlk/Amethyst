using System.Diagnostics;
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
        Debug.Assert(client.Player is not null);

        // Dropping items and shooting arrows share this packet, but are not related to blocks.
        if (digging is not (Digging.Started or Digging.Cancelled or Digging.Finished))
        {
            return;
        }

        var dig = eventDispatcher.Dispatch(client.Player, new Dig(digging, position, face));

        if (dig.IsCancelled)
        {
            // The client might have already removed the block on its side.
            client.Write(new BlockPacket(position, client.Player.World[position]));
        }
    }
}