namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed class TimePacket(long age, long time) : IOutgoingPacket
{
    public int Identifier => 3;

    public long Age => age;

    public long Time => time;
}