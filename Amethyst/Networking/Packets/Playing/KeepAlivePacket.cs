namespace Amethyst.Networking.Packets.Playing;

internal sealed class KeepAlivePacket : IIngoingPacket<KeepAlivePacket>, IOutgoingPacket
{
    static int IIngoingPacket<KeepAlivePacket>.Identifier => 0x00;

    public int Identifier => 0x00;

    public int Payload { get; init; }

    public static KeepAlivePacket Read(MemoryReader reader)
    {
        return new KeepAlivePacket
        {
            Payload = reader.ReadVariableInteger()
        };
    }

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteVariableInteger(Payload);
    }

    public Task HandleAsync(Client client)
    {
        client.MissedKeepAliveCount--;
        return Task.CompletedTask;
    }
}