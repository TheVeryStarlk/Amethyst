using Amethyst.Api;
using Amethyst.Api.Entities;

namespace Amethyst.Protocol.Packets.Playing;

internal sealed class KeepAlivePacket : IIngoingPacket<KeepAlivePacket>, IOutgoingPacket
{
    static int IIngoingPacket<KeepAlivePacket>.Identifier => 0x00;

    public int Identifier => 0x00;

    public required int Payload { get; init; }

    public static KeepAlivePacket Read(MemoryReader reader)
    {
        return new KeepAlivePacket
        {
            Payload = reader.ReadVariableInteger()
        };
    }

    public Task HandleAsync(IServer server, IPlayer player, IClient client)
    {
        client.Idle = DateTimeOffset.Now;
        return Task.CompletedTask;
    }

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteVariableInteger(Payload);
    }
}