using Amethyst.Networking.Ingoing.Packets;

namespace Amethyst.Networking.Ingoing.Processors;

internal interface IProcessor<in TPacket> where TPacket : IIngoingPacket<TPacket>
{
    public void Process(Client client, TPacket packet);
}