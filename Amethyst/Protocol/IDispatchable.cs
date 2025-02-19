using System.Collections.Frozen;
using Amethyst.Components.Protocol;
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
        { MessagePacket.Identifier, static packet => packet.Create<MessagePacket>() },
        { OnGroundPacket.Identifier, static packet => packet.Create<OnGroundPacket>() },
        { PositionPacket.Identifier, static packet => packet.Create<PositionPacket>() },
        { LookPacket.Identifier, static packet => packet.Create<LookPacket>() },
        { PositionLookPacket.Identifier, static packet => packet.Create<PositionLookPacket>() },
        { ConfigurationPacket.Identifier, static packet => packet.Create<ConfigurationPacket>() },
        { TabRequestPacket.Identifier, static packet => packet.Create<TabRequestPacket>() }
    }.ToFrozenDictionary();

    public static IDispatchable? Create(Packet packet)
    {
        return Dictionary.GetValueOrDefault(packet.Identifier)?.Invoke(packet);
    }
}