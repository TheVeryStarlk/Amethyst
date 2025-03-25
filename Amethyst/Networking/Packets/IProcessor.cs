using System.Collections.Frozen;
using Amethyst.Networking.Packets.Play;

namespace Amethyst.Networking.Packets;

internal interface IProcessor
{
    public void Process(Client client);
}

internal static class Processor
{
    // I wonder if there's a better way to do this.
    private static readonly FrozenDictionary<int, Func<Packet, IProcessor>> Dictionary = new Dictionary<int, Func<Packet, IProcessor>>
    {
        { MessagePacket.Identifier, packet => packet.Create<MessagePacket>() },
        { OnGroundPacket.Identifier, packet => packet.Create<OnGroundPacket>() },
        { ConfigurationPacket.Identifier, packet => packet.Create<ConfigurationPacket>() }
    }.ToFrozenDictionary();

    public static IProcessor? Create(Packet packet)
    {
        return Dictionary.GetValueOrDefault(packet.Identifier)?.Invoke(packet);
    }
}