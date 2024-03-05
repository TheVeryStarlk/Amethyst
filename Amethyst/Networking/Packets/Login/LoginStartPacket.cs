namespace Amethyst.Networking.Packets.Login;

internal sealed class LoginStartPacket : IIngoingPacket<LoginStartPacket>
{
    public static int Identifier => 0x00;

    public required string Username { get; init; }

    public static LoginStartPacket Read(MemoryReader reader)
    {
        return new LoginStartPacket
        {
            Username = reader.ReadVariableString()
        };
    }
}