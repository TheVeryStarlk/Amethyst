namespace Amethyst.Abstractions.Protocol.Packets.Login;

public sealed class LoginStartPacket : IIngoingPacket<LoginStartPacket>
{
    public static int Identifier => 0;

    public required string Username { get; init; }

    static LoginStartPacket IIngoingPacket<LoginStartPacket>.Create(SpanReader reader)
    {
        return new LoginStartPacket
        {
            Username = reader.ReadVariableString()
        };
    }
}