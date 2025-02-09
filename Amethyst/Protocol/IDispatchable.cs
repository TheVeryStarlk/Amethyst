using System.Collections.Frozen;
using Amethyst.Components.Entities;
using Amethyst.Components.Protocol;
using Amethyst.Eventing;
using Amethyst.Protocol.Packets.Play;

namespace Amethyst.Protocol;

internal interface IDispatchable
{
    public Task DispatchAsync(IPlayer player, EventDispatcher eventDispatcher, CancellationToken cancellationToken);
}

internal static class Dispatchable
{
    public static FrozenDictionary<int, Func<Packet, IDispatchable>> Registered { get; } = new Dictionary<int, Func<Packet, IDispatchable>>
    {
        { MessagePacket.Identifier, static packet => packet.Create<MessagePacket>() },
        { OnGroundPacket.Identifier, static packet => packet.Create<OnGroundPacket>() },
        { PositionPacket.Identifier, static packet => packet.Create<PositionPacket>() },
        { LookPacket.Identifier, static packet => packet.Create<LookPacket>() },
        { PositionLookPacket.Identifier, static packet => packet.Create<PositionLookPacket>() }
    }.ToFrozenDictionary();
}