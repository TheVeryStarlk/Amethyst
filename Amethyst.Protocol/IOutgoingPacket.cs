namespace Amethyst.Protocol;

public interface IOutgoingPacket
{
    public int Identifier { get; }

    public int Length { get; }

    public void Write(Span<byte> span);
}