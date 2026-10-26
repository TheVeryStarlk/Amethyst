using System.Collections.Frozen;
using Amethyst.Eventing;

namespace Amethyst.Networking.Packets;

internal interface IProcessor
{
    public void Process(Client client, Dispatcher dispatcher);
}

internal sealed class EmptyProcessor : IProcessor
{
    public static EmptyProcessor Instance { get; } = new();

    public void Process(Client client, Dispatcher dispatcher)
    {
        // Will remove this later.
    }
}

internal static partial class PacketExtensions
{
    private static readonly FrozenDictionary<(State State, int Identifier), Func<Packet, IProcessor>> creators = Creators().ToFrozenDictionary();

    public static IProcessor Create(this Packet packet, State state)
    {
        if (creators.TryGetValue((state, packet.Identifier), out var create))
        {
            return create(packet);
        }

        // Unhandled play packets are ignored for now.
        return state is State.Play
            ? EmptyProcessor.Instance
            : throw new ArgumentOutOfRangeException(nameof(packet), packet.Identifier, "Unknown packet identifier.");
    }

    // Generated from every ingoing packet.
    private static partial Dictionary<(State State, int Identifier), Func<Packet, IProcessor>> Creators();
}