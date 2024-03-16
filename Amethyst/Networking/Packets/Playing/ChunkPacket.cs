using Amethyst.Api.Levels;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class ChunkPacket : IOutgoingPacket
{
    public int Identifier => 0x21;

    public required IChunk Chunk { get; init; }

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteInteger((int) Chunk.Position.X);
        writer.WriteInteger((int) Chunk.Position.Z);
        writer.WriteBoolean(true);

        var serializedChunk = Chunk.Serialize();
        writer.WriteUnsignedShort(serializedChunk.Bitmask);
        writer.WriteVariableInteger(serializedChunk.Payload.Length);
        writer.Write(serializedChunk.Payload);
    }
}