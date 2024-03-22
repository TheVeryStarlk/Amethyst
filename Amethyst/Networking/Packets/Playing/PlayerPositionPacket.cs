using Amethyst.Api.Components;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class PlayerPositionPacket : IIngoingPacket<PlayerPositionPacket>
{
    public static int Identifier => 0x04;

    public required VectorF Position { get; init; }

    public required bool OnGround { get; init; }

    public static PlayerPositionPacket Read(MemoryReader reader)
    {
        return new PlayerPositionPacket
        {
            Position = new VectorF(
                reader.ReadDouble(),
                reader.ReadDouble(),
                reader.ReadDouble()),

            OnGround = reader.ReadBoolean()
        };
    }

    public Task HandleAsync(Client client)
    {
        client.Player!.Position = Position;
        client.Player.OnGround = OnGround;
        return Task.CompletedTask;
    }
}