using Amethyst.Abstractions.Networking.Packets.Status;
using Amethyst.Eventing.Client;
using Amethyst.Networking.Packets;
using Amethyst.Networking.Packets.Status;

namespace Amethyst.Networking.Processors;

internal sealed class StatusProcessor : IProcessor
{
    public static void Process(Client client, Packet packet)
    {
        if (packet.Identifier == StatusRequestPacket.Identifier)
        {
            var status = client.EventDispatcher.Dispatch(client, new Status());
            client.Write(new StatusResponsePacket(status.Name, status.Numerical, status.Maximum, status.Online, status.Description, status.Favicon));
        }
        else
        {
            client.Write(new PongPacket(packet.Create<PingPacket>().Magic));
            client.Stop();
        }
    }
}