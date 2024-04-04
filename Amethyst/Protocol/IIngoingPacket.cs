namespace Amethyst.Protocol;

public interface IIngoingPacket<out TSelf> where TSelf : IIngoingPacket<TSelf>
{
    public static abstract int Identifier { get; }

    public static abstract TSelf Read(MemoryReader reader);
}