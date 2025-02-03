namespace Amethyst.Abstractions.Protocol;

public interface IOutgoingPacket
{
    public int Identifier { get; }

    internal int Length { get; }

    internal void Write(ref SpanWriter writer);
}