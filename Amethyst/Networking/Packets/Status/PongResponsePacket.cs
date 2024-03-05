namespace Amethyst.Networking.Packets.Status;

internal sealed class PongResponsePacket: IOutgoingPacket
{
    public int Identifier => 0x01;

    public required long Payload { get; init; }

    public int CalculateLength()
    {
        return sizeof(long);
    }

    public int Write(ref MemoryWriter writer)
    {
        writer.WriteLong(Payload);
        return writer.Position;
    }
}