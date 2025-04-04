using Amethyst.Networking.Packets;

namespace Amethyst.Networking.Processors;

internal interface IProcessor
{
    public static abstract void Process(Client client, Packet packet);
}