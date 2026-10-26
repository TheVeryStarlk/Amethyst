using Amethyst.Abstractions.Packets.Status;
using Amethyst.Eventing;
using Amethyst.Abstractions.Eventing.Client;

namespace Amethyst.Networking.Packets.Status;

internal sealed partial class StatusRequestPacket : IIngoingPacket<StatusRequestPacket>, IProcessor
{
    public static State State => State.Status;

    public static int Identifier => 0;

    public void Process(Client client, Dispatcher dispatcher)
    {
        var request = dispatcher.Dispatch(client, new Request());
        client.Write(new StatusResponsePacket(request.Name, request.Numerical, request.Maximum, request.Online, request.Description, request.Favicon));
    }
}