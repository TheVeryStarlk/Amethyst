using Amethyst.Abstractions.Packets;

namespace Amethyst.Networking.Serializers;

internal interface ISerializer
{
    public int Length { get; }

    public void Write(Span<byte> span);
}

internal interface ISerializer<in T, out TSerializer> : ISerializer where T : IOutgoingPacket where TSerializer : ISerializer
{
    public static abstract TSerializer Create(T packet);
}

internal static partial class OutgoingPacketExtensions
{
    // Generated from every serializer.
    public static partial ISerializer Create(this IOutgoingPacket instance);
}