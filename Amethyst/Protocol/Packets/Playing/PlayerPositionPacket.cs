using Amethyst.Api;
using Amethyst.Api.Components;
using Amethyst.Api.Entities;

namespace Amethyst.Protocol.Packets.Playing;

internal sealed class PlayerPositionPacket : IIngoingPacket<PlayerPositionPacket>
{
    public static int Identifier => 0x04;

    public required VectorF Vector { get; init; }

    public required bool OnGround { get; init; }

    public static PlayerPositionPacket Read(MemoryReader reader)
    {
        return new PlayerPositionPacket
        {
            Vector = new VectorF(
                reader.ReadDouble(),
                reader.ReadDouble(),
                reader.ReadDouble()),

            OnGround = reader.ReadBoolean()
        };
    }

    public Task HandleAsync(IServer server, IPlayer player, IClient client)
    {
        player.Vector = Vector;
        player.OnGround = OnGround;
        return Task.CompletedTask;
    }
}