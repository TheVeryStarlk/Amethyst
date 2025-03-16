namespace Amethyst.Networking.Packets.Play;

internal sealed record ConfigurationPacket(string Locale, byte ViewDistance) : IIngoingPacket<ConfigurationPacket>, IProcessor
{
    public static int Identifier => 21;

    public static ConfigurationPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new ConfigurationPacket(reader.ReadVariableString(), reader.ReadByte());
    }

    public void Process(Client client)
    {
        client.Player!.Locale = Locale;
        client.Player.ViewDistance = ViewDistance;
    }
}