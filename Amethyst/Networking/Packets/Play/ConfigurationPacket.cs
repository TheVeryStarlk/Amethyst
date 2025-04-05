namespace Amethyst.Networking.Packets.Play;

internal sealed class ConfigurationPacket(string locale, byte viewDistance) : IIngoingPacket<ConfigurationPacket>
{
    public static int Identifier => 21;

    public string Locale => locale;

    public byte ViewDistance => viewDistance;

    public static ConfigurationPacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new ConfigurationPacket(reader.ReadVariableString(), reader.ReadByte());
    }
}