namespace Amethyst.Protocol.Packets.Login;

public sealed record LoginStartPacket(string Username) : IIngoingPacket<LoginStartPacket>
{
    public static int Identifier => 0;

    static LoginStartPacket IIngoingPacket<LoginStartPacket>.Create(SpanReader reader)
    {
        return new LoginStartPacket(reader.ReadVariableString());
    }
}