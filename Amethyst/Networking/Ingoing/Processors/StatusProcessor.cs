using Amethyst.Abstractions.Eventing.Client;
using Amethyst.Abstractions.Packets.Status;
using Amethyst.Eventing;
using Amethyst.Networking.Ingoing.Packets.Status;

namespace Amethyst.Networking.Ingoing.Processors;

internal sealed class StatusProcessor(Dispatcher dispatcher) : IProcessor<StatusRequestPacket>, IProcessor<PingPacket>
{
    public void Process(Client client, StatusRequestPacket packet)
    {
        var request = dispatcher.Dispatch(client, new Request());
        client.Write(new StatusResponsePacket(request.Name, request.Numerical, request.Maximum, request.Online, request.Description, request.Favicon));
    }

    public void Process(Client client, PingPacket packet)
    {
        client.Write(new PongPacket(packet.Magic));
        client.Stop();
    }
}