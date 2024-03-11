namespace Amethyst.Networking.Packets.Playing;

internal sealed class PlayerLookPacket : IIngoingPacket<PlayerLookPacket>
{
    public static int Identifier => 0x05;

    public required float Yaw { get; init; }

    public required float Pitch { get; init; }

    public required bool OnGround { get; init; }

    public static PlayerLookPacket Read(MemoryReader reader)
    {
        return new PlayerLookPacket
        {
            Yaw = reader.ReadFloat(),
            Pitch = reader.ReadFloat(),
            OnGround = reader.ReadBoolean()
        };
    }

    public Task HandleAsync(Client client)
    {
        client.Player!.Yaw = Yaw;
        client.Player.Pitch = Pitch;
        client.Player.OnGround = OnGround;
        return Task.CompletedTask;
    }
}