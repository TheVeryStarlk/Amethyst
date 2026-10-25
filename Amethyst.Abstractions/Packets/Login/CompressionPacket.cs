namespace Amethyst.Abstractions.Packets.Login;

internal sealed class CompressionPacket(int threshold) : IOutgoingPacket
{
    public int Identifier => 3;

    public int Threshold => threshold;
}