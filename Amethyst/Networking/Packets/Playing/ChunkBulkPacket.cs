using Amethyst.Api.Levels;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class ChunkBulkPacket : IOutgoingPacket
{
    public int Identifier => 0x26;

    public required IEnumerable<IChunk> Chunks { get; init; }

    private (byte[] Payload, ushort Bitmask)[]? serializedChunks;

    public int CalculateLength()
    {
        var length = 0;
        var index = 0;

        serializedChunks = new (byte[] Payload, ushort Bitmask)[Chunks.Count()];

        foreach (var chunk in Chunks)
        {
            var serialized = chunk.Serialize();
            serializedChunks[index++] = serialized;
            length += serialized.Payload.Length;
        }

        return length;
    }

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteBoolean(true);
        writer.WriteVariableInteger(Chunks.Count());

        var index = 0;

        foreach (var chunk in Chunks)
        {
            writer.WriteInteger((int) chunk.Position.X);
            writer.WriteInteger((int) chunk.Position.Z);
            writer.WriteUnsignedShort(serializedChunks![index++].Bitmask);
        }

        foreach (var chunk in serializedChunks!)
        {
            writer.Write(chunk.Payload);
        }
    }
}