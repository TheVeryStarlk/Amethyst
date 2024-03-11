using Amethyst.Extensions;

namespace Amethyst.Networking.Packets.Status;

internal sealed class PingRequestPacket : IIngoingPacket<PingRequestPacket>
{
    public static int Identifier => 0x01;

    public required long Payload { get; init; }

    public static PingRequestPacket Read(MemoryReader reader)
    {
        return new PingRequestPacket
        {
            Payload = reader.ReadLong()
        };
    }

    public async Task HandleAsync(Client client)
    {
        await client.Transport.Output.WritePacketAsync(
            new PongResponsePacket
            {
                Payload = Payload
            });
    }
}