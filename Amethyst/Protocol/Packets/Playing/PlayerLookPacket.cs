using Amethyst.Api;
using Amethyst.Api.Entities;

namespace Amethyst.Protocol.Packets.Playing;

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

    public Task HandleAsync(IServer server, IPlayer player, IClient client)
    {
        player.Yaw = Yaw;
        player.Pitch = Pitch;
        player.OnGround = OnGround;
        return Task.CompletedTask;
    }
}