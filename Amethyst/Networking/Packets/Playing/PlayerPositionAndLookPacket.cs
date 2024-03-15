using Amethyst.Api.Commands;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class PlayerPositionAndLookPacket : IIngoingPacket<PlayerPositionAndLookPacket>, IOutgoingPacket
{
    static int IIngoingPacket<PlayerPositionAndLookPacket>.Identifier => 0x06;

    public int Identifier => 0x08;

    public required VectorF Position { get; init; }

    public float Yaw { get; init; }

    public float Pitch { get; init; }

    public bool OnGround { get; init; }

    public static PlayerPositionAndLookPacket Read(MemoryReader reader)
    {
        return new PlayerPositionAndLookPacket
        {
            Position = new VectorF(
                reader.ReadFloat(),
                reader.ReadFloat(),
                reader.ReadFloat()),

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

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteDouble(Position.X);
        writer.WriteDouble(Position.Y);
        writer.WriteDouble(Position.Z);
        writer.WriteFloat(Yaw);
        writer.WriteFloat(Pitch);
        writer.WriteByte(0);
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