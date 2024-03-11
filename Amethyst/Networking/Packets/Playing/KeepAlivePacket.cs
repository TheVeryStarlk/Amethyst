using Amethyst.Utilities;

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

    public int CalculateLength()
    {
        return VariableInteger.GetBytesCount(Payload);
    }

    public int Write(ref MemoryWriter writer)
    {
        writer.WriteVariableInteger(Payload);
        return writer.Position;
    }

    public Task HandleAsync(Client client)
    {
        client.KeepAliveCount--;
        return Task.CompletedTask;
    }
}