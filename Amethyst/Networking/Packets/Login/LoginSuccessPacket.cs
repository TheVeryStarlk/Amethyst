namespace Amethyst.Networking.Packets.Login;

internal sealed class LoginSuccessPacket : IOutgoingPacket
{
    public int Identifier => 0x02;

    public required Guid Guid { get; init; }

    public required string Username { get; init; }

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteVariableString(Guid.ToString());
        writer.WriteVariableString(Username);
    }
}