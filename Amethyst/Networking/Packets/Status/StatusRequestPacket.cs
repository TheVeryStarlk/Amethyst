using Amethyst.Abstractions.Networking.Packets.Status;
using Amethyst.Eventing;
using Amethyst.Eventing.Client;

namespace Amethyst.Networking.Packets.Status;

internal sealed class StatusRequestPacket : IIngoingPacket<StatusRequestPacket>, IProcessor
{
    public static int Identifier => 0;

    public static StatusRequestPacket Create(ReadOnlySpan<byte> span)
    {
        return new StatusRequestPacket();
    }

    public void Process(Client client, EventDispatcher eventDispatcher)
    {
        var request = eventDispatcher.Dispatch(client, new Request());
        client.Write(new StatusResponsePacket(request.Name, request.Numerical, request.Maximum, request.Online, request.Description, request.Favicon));
    }
}