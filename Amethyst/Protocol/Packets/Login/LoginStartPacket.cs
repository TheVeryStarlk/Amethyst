namespace Amethyst.Protocol.Packets.Login;

internal sealed record LoginStartPacket(string Username) : LoginStartPacketBase(Username), ICreatable<LoginStartPacketBase>
{
    public static LoginStartPacketBase Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new LoginStartPacket(reader.ReadVariableString());
    }
}