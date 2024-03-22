using Amethyst.Api.Entities;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class SpawnPlayerPacket : IOutgoingPacket
{
    public int Identifier => 0x0C;

    public required IPlayer Player { get; init; }

    public void Write(ref MemoryWriter writer)
    {
        Console.WriteLine($"{Player.Position.X} {Player.Position.Y} {Player.Position.Z}");
        writer.WriteVariableInteger(Player.Identifier);
        writer.WriteGuid(Player.Guid);
        writer.WriteFixedPointInteger((int) Player.Position.X);
        writer.WriteFixedPointInteger((int) Player.Position.Y);
        writer.WriteFixedPointInteger((int) Player.Position.Z);
        writer.WriteByte((byte) (Player.Yaw / 256));
        writer.WriteByte((byte) (Player.Pitch / 256));
        writer.WriteShort(0);
        writer.WriteByte(127);
    }
}