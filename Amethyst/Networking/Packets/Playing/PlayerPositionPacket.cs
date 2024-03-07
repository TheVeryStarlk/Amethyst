using System.Numerics;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class PlayerPositionPacket : IIngoingPacket<PlayerPositionPacket>
{
    public static int Identifier => 0x04;

    public required Vector3 Position { get; init; }

    public required bool OnGround { get; init; }

    public static PlayerPositionPacket Read(MemoryReader reader)
    {
        var position = new Vector3(
            (float) reader.ReadDouble(),
            (float) reader.ReadDouble(),
            (float) reader.ReadDouble());

        return new PlayerPositionPacket
        {
            Position = position,
            OnGround = reader.ReadBoolean()
        };
    }

    public Task HandleAsync(MinecraftClient client)
    {
        client.Player!.Position = Position;
        client.Player.OnGround = OnGround;
        return Task.CompletedTask;
    }
}