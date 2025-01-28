namespace Amethyst.Protocol;

internal interface IIngoingPacket<out T> where T : IIngoingPacket<T>
{
    public static abstract int Identifier { get; }

    public static abstract T Create();
}