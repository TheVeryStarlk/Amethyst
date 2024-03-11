using Amethyst.Api.Entities;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class PlayerPositionAndLookPacket : IIngoingPacket<PlayerPositionAndLookPacket>, IOutgoingPacket
{
    static int IIngoingPacket<PlayerPositionAndLookPacket>.Identifier => 0x06;

    public int Identifier => 0x08;

    public required Position Position { get; set; }

    public float Yaw { get; set; }

    public float Pitch { get; set; }

    public bool OnGround { get; set; }

    public static PlayerPositionAndLookPacket Read(MemoryReader reader)
    {
        var position = new Position(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat());

        return new PlayerPositionAndLookPacket
        {
            Position = position,
            Yaw = reader.ReadFloat(),
            Pitch = reader.ReadFloat(),
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
        writer.WriteFloat(Yaw);
        writer.WriteFloat(Pitch);
        writer.WriteByte(0);

        return writer.Position;
    }

    public Task HandleAsync(Client client)
    {
        client.Player!.Position = Position;
        client.Player.Yaw = Yaw;
        client.Player.Pitch = Pitch;
        client.Player.OnGround = OnGround;
        return Task.CompletedTask;
    }
}