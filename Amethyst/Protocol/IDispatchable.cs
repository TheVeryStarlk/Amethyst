using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using Amethyst.Components.Protocol;
using Amethyst.Entities;
using Amethyst.Eventing;
using Amethyst.Protocol.Packets.Play;

namespace Amethyst.Protocol;

internal interface IDispatchable
{
    public Task DispatchAsync(Player player, EventDispatcher eventDispatcher, CancellationToken cancellationToken);
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
        { PositionLookPacket.Identifier, static packet => packet.Create<PositionLookPacket>() }
    }.ToFrozenDictionary();

    public static bool TryCreate(Packet packet, [NotNullWhen(true)] out IDispatchable? dispatchable)
    {
        if (Dictionary.TryGetValue(packet.Identifier, out var factory))
        {
            dispatchable = factory(packet);
            return true;
        }

        dispatchable = null;
        return false;
    }
}