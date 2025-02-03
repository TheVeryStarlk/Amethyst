namespace Amethyst.Protocol;

internal interface IOutgoingPacket
{
    public int Identifier { get; }

    public int Length { get; }

    public void Write(ref SpanWriter writer);
}