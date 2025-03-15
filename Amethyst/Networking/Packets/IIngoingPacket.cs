using System.Collections.Frozen;
using Amethyst.Networking.Packets.Handshake;
using Amethyst.Networking.Packets.Status;

namespace Amethyst.Networking.Packets;

internal interface IIngoingPacket
{
    public void Process(Client client);
}

internal interface IIngoingPacket<out T> : IIngoingPacket where T : IIngoingPacket<T>
{
    public static abstract int Identifier { get; }

    public static abstract T Create(ReadOnlySpan<byte> span);
}

internal static class IngoingPacket
{
    public static readonly FrozenDictionary<int, Func<ReadOnlySpan<byte>, IIngoingPacket>> Handshake = new Dictionary<int, Func<ReadOnlySpan<byte>, IIngoingPacket>>
    {
        { HandshakePacket.Identifier, HandshakePacket.Create }
    }.ToFrozenDictionary();

    public static readonly FrozenDictionary<int, Func<ReadOnlySpan<byte>, IIngoingPacket>> Status = new Dictionary<int, Func<ReadOnlySpan<byte>, IIngoingPacket>>
    {
        { StatusRequestPacket.Identifier, StatusRequestPacket.Create },
        { PingPacket.Identifier, PingPacket.Create }
    }.ToFrozenDictionary();

    public static readonly FrozenDictionary<int, Func<ReadOnlySpan<byte>, IIngoingPacket>> Login = new Dictionary<int, Func<ReadOnlySpan<byte>, IIngoingPacket>>
    {
    }.ToFrozenDictionary();

    public static readonly FrozenDictionary<int, Func<ReadOnlySpan<byte>, IIngoingPacket>> Play = new Dictionary<int, Func<ReadOnlySpan<byte>, IIngoingPacket>>
    {
    }.ToFrozenDictionary();
}