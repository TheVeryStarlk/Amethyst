using System.Collections.Frozen;
using Amethyst.Entities;
using Amethyst.Eventing;
using Amethyst.Protocol.Packets.Play;

namespace Amethyst.Protocol;

internal interface IDispatchable
{
    public void Dispatch(Player player, EventDispatcher eventDispatcher);
}

internal static class Dispatchable
{
    // I wonder if there's a better way to do this.
    private static readonly FrozenDictionary<int, Func<Packet, IDispatchable>> Dictionary = new Dictionary<int, Func<Packet, IDispatchable>>
    {
        { MessagePacket.Identifier, packet => packet.Create<MessagePacket>() },
        { OnGroundPacket.Identifier, packet => packet.Create<OnGroundPacket>() },
        { PositionPacket.Identifier, packet => packet.Create<PositionPacket>() },
        { LookPacket.Identifier, packet => packet.Create<LookPacket>() },
        { PositionLookPacket.Identifier, packet => packet.Create<PositionLookPacket>() },
        { ConfigurationPacket.Identifier, packet => packet.Create<ConfigurationPacket>() },
        { TabRequestPacket.Identifier, packet => packet.Create<TabRequestPacket>() },
        { PlacementPacket.Identifier, packet => packet.Create<PlacementPacket>() },
        { DiggingPacket.Identifier, packet => packet.Create<DiggingPacket>() }
    }.ToFrozenDictionary();

    public static IDispatchable? Create(Packet packet)
    {
        return Dictionary.GetValueOrDefault(packet.Identifier)?.Invoke(packet);
    }
}