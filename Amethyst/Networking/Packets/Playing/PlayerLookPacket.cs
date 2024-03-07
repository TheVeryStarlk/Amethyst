using System.Numerics;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class PlayerLookPacket : IIngoingPacket<PlayerLookPacket>
{
    public static int Identifier => 0x05;

    public required Vector2 Rotation { get; init; }

    public required bool OnGround { get; init; }

    public static PlayerLookPacket Read(MemoryReader reader)
    {
        return new PlayerLookPacket
        {
            Rotation = new Vector2(reader.ReadFloat(), reader.ReadFloat()),
            OnGround = reader.ReadBoolean()
        };
    }

    public Task HandleAsync(MinecraftClient client)
    {
        client.Player!.Rotation = Rotation;
        client.Player.OnGround = OnGround;
        return Task.CompletedTask;
    }
}