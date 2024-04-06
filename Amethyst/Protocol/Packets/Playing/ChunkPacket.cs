using Amethyst.Worlds;

namespace Amethyst.Protocol.Packets.Playing;

internal sealed class ChunkPacket : IOutgoingPacket
{
    public int Identifier => 0x21;

    public required Chunk Chunk { get; init; }

    public bool Unload { get; init; }

    public int CalculateLength()
    {
        // (8192 + 2048 + 2048) * 16 = 196608
        return Unload ? short.MaxValue : 196608;
    }

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteInteger((int) Chunk.Position.X);
        writer.WriteInteger((int) Chunk.Position.Z);
        writer.WriteBoolean(true);

        if (Unload)
        {
            writer.WriteUnsignedShort(0);
            writer.WriteVariableInteger(Array.Empty<byte>().Length);
            writer.Write(Array.Empty<byte>());
        }
        else
        {
            var serialized = Chunk.Serialize();

            writer.WriteUnsignedShort(serialized.Bitmask);
            writer.WriteVariableInteger(serialized.Payload.Length);
            writer.Write(serialized.Payload);
        }
    }
}