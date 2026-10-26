using System.Collections.Frozen;
using Amethyst.Eventing;
using Amethyst.Networking.Packets.Handshake;
using Amethyst.Networking.Packets.Login;
using Amethyst.Networking.Packets.Play;
using Amethyst.Networking.Packets.Status;

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

internal static class PacketExtensions
{
    private static readonly FrozenDictionary<(State State, int Identifier), Func<Packet, IProcessor>> creators;

    static PacketExtensions()
    {
        var dictionary = new Dictionary<(State State, int Identifier), Func<Packet, IProcessor>>
        {
            // Handshake.
            { (State.Handshake, HandshakePacket.Identifier), static packet => packet.Create<HandshakePacket>() },
            
            // Status.
            { (State.Status, StatusRequestPacket.Identifier), static packet => packet.Create<StatusRequestPacket>() },
            { (State.Status, PingPacket.Identifier), static packet => packet.Create<PingPacket>() },
            
            // Login.
            { (State.Login, StartPacket.Identifier), static packet => packet.Create<StartPacket>() },
            { (State.Login, EncryptionResponsePacket.Identifier), static packet => packet.Create<EncryptionResponsePacket>() },
            
            // Play.
            { (State.Play, MessagePacket.Identifier), static packet => packet.Create<MessagePacket>() },
            { (State.Play, GroundPacket.Identifier), static packet => packet.Create<GroundPacket>() },
            { (State.Play, PositionPacket.Identifier), static packet => packet.Create<PositionPacket>() },
            { (State.Play, LookPacket.Identifier), static packet => packet.Create<LookPacket>() },
            { (State.Play, PositionLookPacket.Identifier), static packet => packet.Create<PositionLookPacket>() },
            { (State.Play, DiggingPacket.Identifier), static packet => packet.Create<DiggingPacket>() },
            { (State.Play, PlacementPacket.Identifier), static packet => packet.Create<PlacementPacket>() },
            { (State.Play, HeldItemChangePacket.Identifier), static packet => packet.Create<HeldItemChangePacket>() },
            { (State.Play, InventoryActionPacket.Identifier), static packet => packet.Create<InventoryActionPacket>() },
            { (State.Play, TabRequestPacket.Identifier), static packet => packet.Create<TabRequestPacket>() },
            { (State.Play, ConfigurationPacket.Identifier), static packet => packet.Create<ConfigurationPacket>() },
            { (State.Play, StatusPacket.Identifier), static packet => packet.Create<StatusPacket>() }
        };

        creators = dictionary.ToFrozenDictionary();
    }

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
}