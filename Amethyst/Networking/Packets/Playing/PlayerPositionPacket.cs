using Amethyst.Api.Entities;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class PlayerPositionPacket : IIngoingPacket<PlayerPositionPacket>
{
    public static int Identifier => 0x04;

    public required Position Position { get; set; }

    public required bool OnGround { get; init; }

    public static PlayerPositionPacket Read(MemoryReader reader)
    {
        var position = new Position(reader.ReadFloat(), reader.ReadFloat(), reader.ReadFloat());

        return new PlayerPositionPacket
        {
            Position = position,
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