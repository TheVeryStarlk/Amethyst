using Amethyst.Api.Entities;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class PlayerPositionAndLookPacket : IOutgoingPacket
{
    public static int Identifier => 0x08;

    public required IPlayer Player { get; init; }

    public int CalculateLength()
    {
        return sizeof(double)
               + sizeof(double)
               + sizeof(double)
               + sizeof(float)
               + sizeof(float)
               + sizeof(byte);
    }

    public int Write(ref MemoryWriter writer)
    {
        writer.WriteDouble(Player.Position.X);
        writer.WriteDouble(Player.Position.Y);
        writer.WriteDouble(Player.Position.Z);
        writer.WriteFloat(Player.Rotation.X);
        writer.WriteFloat(Player.Rotation.Y);
        writer.WriteByte(0);

        return writer.Position;
    }
}