namespace Amethyst.Networking.Packets;

internal interface IPacket
{
    public static abstract int Identifier { get; }
}

internal interface IIngoingPacket<out TSelf> : IPacket where TSelf : IIngoingPacket<TSelf>
{
    public static abstract TSelf Read(MemoryReader reader);
}

internal interface IOutgoingPacket : IPacket
{
    public int CalculateLength();

    public int Write(ref MemoryWriter writer);
}