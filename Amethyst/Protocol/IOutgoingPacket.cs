namespace Amethyst.Protocol;

public interface IOutgoingPacket
{
    public int Identifier { get; }

    public int Length { get; }

    internal void Write(ref SpanWriter writer);
}