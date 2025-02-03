namespace Amethyst.Protocol.Packets.Login;

internal sealed record LoginStartPacket(string Username) : IIngoingPacket<LoginStartPacket>
{
    public static int Identifier => 0;

    public static LoginStartPacket Create(SpanReader reader)
    {
        return new LoginStartPacket(reader.ReadVariableString());
    }
}