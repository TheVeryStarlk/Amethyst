using Amethyst.Api.Levels;
using Amethyst.Utilities;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class ChunkPacket : IOutgoingPacket
{
    public int Identifier => 0x21;

    public required IChunk Chunk { get; init; }

    private (byte[] payload, ushort bitmask)? serializedChunk;

    public int CalculateLength()
    {
        serializedChunk = Chunk.Serialize();

        return sizeof(int)
               + sizeof(int)
               + sizeof(bool)
               + sizeof(ushort)
               + VariableInteger.GetBytesCount(serializedChunk.Value.payload.Length)
               + serializedChunk.Value.payload.Length;
    }

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteInteger((int) Chunk.Position.X);
        writer.WriteInteger((int) Chunk.Position.Z);
        writer.WriteBoolean(true);
        writer.WriteUnsignedShort(serializedChunk!.Value.bitmask);
        writer.WriteVariableInteger(serializedChunk!.Value.payload.Length);
        writer.Write(serializedChunk!.Value.payload);
    }
}