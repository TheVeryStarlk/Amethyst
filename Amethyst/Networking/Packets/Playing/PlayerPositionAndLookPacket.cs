using System.Numerics;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class PlayerPositionAndLookPacket : IIngoingPacket<PlayerPositionAndLookPacket>, IOutgoingPacket
{
    static int IIngoingPacket<PlayerPositionAndLookPacket>.Identifier => 0x06;

    public int Identifier => 0x08;

    public required Vector3 Position { get; set; }

    public required Vector2 Rotation { get; set; }

    public bool OnGround { get; set; }

    public static PlayerPositionAndLookPacket Read(MemoryReader reader)
    {
        var position = new Vector3(
            (float) reader.ReadDouble(),
            (float) reader.ReadDouble(),
            (float) reader.ReadDouble());

        return new PlayerPositionAndLookPacket
        {
            Position = position,
            Rotation = new Vector2(reader.ReadFloat(), reader.ReadFloat()),
            OnGround = reader.ReadBoolean()
        };
    }

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
        writer.WriteDouble(Position.X);
        writer.WriteDouble(Position.Y);
        writer.WriteDouble(Position.Z);
        writer.WriteFloat(Rotation.X);
        writer.WriteFloat(Rotation.Y);
        writer.WriteByte(0);

        return writer.Position;
    }

    public Task HandleAsync(MinecraftClient client)
    {
        client.Player!.Position = Position;
        client.Player.Rotation = Rotation;
        client.Player.OnGround = OnGround;
        return Task.CompletedTask;
    }
}