namespace Amethyst.Networking.Packets.Playing;

internal sealed class OnGroundPacket : IIngoingPacket<OnGroundPacket>
{
    public static int Identifier => 0x03;

    public required bool OnGround { get; init; }

    public static OnGroundPacket Read(MemoryReader reader)
    {
        return new OnGroundPacket
        {
            OnGround = reader.ReadBoolean()
        };
    }

    public Task HandleAsync(Client client)
    {
        client.Player!.OnGround = OnGround;
        return Task.CompletedTask;
    }
}