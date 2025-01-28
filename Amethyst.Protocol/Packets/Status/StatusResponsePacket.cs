namespace Amethyst.Protocol.Packets.Status;

public sealed class StatusResponsePacket : IOutgoingPacket
{
    public int Identifier => 0;

    public void Write()
    {
        // Soon.
    }
}