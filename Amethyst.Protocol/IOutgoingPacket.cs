namespace Amethyst.Protocol;

internal interface IOutgoingPacket
{
    public int Identifier { get; }

    public void Write();
}