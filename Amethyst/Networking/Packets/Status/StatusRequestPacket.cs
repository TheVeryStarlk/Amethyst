using Amethyst.Abstractions.Networking.Packets.Status;
using Amethyst.Eventing.Client;

namespace Amethyst.Networking.Packets.Status;

internal sealed class StatusRequestPacket : IIngoingPacket<StatusRequestPacket>, IProcessor
{
    public static int Identifier => 0;

    public static StatusRequestPacket Create(ReadOnlySpan<byte> span)
    {
        return new StatusRequestPacket();
    }

    public void Process(Client client)
    {
        var status = client.EventDispatcher.Dispatch(client, new StatusRequest());
        client.Write(new StatusResponsePacket(status.Name, status.Numerical, status.Maximum, status.Online, status.Description, status.Favicon));
    }
}