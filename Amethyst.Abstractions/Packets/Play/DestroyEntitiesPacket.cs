namespace Amethyst.Abstractions.Packets.Play;

public sealed class DestroyEntitiesPacket(params int[] uniques) : IOutgoingPacket
{
    public int Identifier => 19;

    public int[] Uniques => uniques;
}