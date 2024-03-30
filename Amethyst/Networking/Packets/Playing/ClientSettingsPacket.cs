namespace Amethyst.Networking.Packets.Playing;

internal sealed class ClientSettingsPacket : IIngoingPacket<ClientSettingsPacket>
{
    public static int Identifier => 0x15;

    public required string Locale { get; init; }

    public required byte ViewDistance { get; init; }

    public required byte ChatMode { get; init; }

    public required bool ChatHasColor { get; init; }

    public required byte DisplayedSkinParts { get; init; }

    public static ClientSettingsPacket Read(MemoryReader reader)
    {
        return new ClientSettingsPacket
        {
            Locale = reader.ReadVariableString(),
            ViewDistance = reader.ReadByte(),
            ChatMode = reader.ReadByte(),
            ChatHasColor = reader.ReadBoolean(),
            DisplayedSkinParts = reader.ReadByte()
        };
    }

    public Task HandleAsync(Client client)
    {
        client.Player!.ViewDistance = ViewDistance;
        return Task.CompletedTask;
    }
}