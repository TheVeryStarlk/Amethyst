using Amethyst.Api.Levels;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class ChunkPacket : IOutgoingPacket
{
    public int Identifier => 0x21;

    public required IChunk Chunk { get; init; }

    public bool Unload { get; init; }

    private (byte[] Payload, ushort Bitmask)? serializedChunk;

    public int CalculateLength()
    {
        serializedChunk = Chunk.Serialize();
        return serializedChunk.Value.Payload.Length;
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
            writer.WriteUnsignedShort(serializedChunk!.Value.Bitmask);
            writer.WriteVariableInteger(serializedChunk!.Value.Payload.Length);
            writer.Write(serializedChunk!.Value.Payload);
        }
    }
}