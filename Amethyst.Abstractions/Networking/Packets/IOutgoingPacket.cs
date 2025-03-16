namespace Amethyst.Abstractions.Networking.Packets;

// I think only play packets should be public.
public interface IOutgoingPacket
{
    public int Identifier { get; }
}