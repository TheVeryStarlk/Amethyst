namespace Amethyst.Protocol.Packets.Login;

public sealed record LoginStartPacket(string Username) : IIngoingPacket<LoginStartPacket>
{
    public static int Identifier => 0;

    public static LoginStartPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new LoginStartPacket(reader.ReadVariableString());
    }
}