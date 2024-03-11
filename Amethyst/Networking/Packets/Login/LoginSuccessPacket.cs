using Amethyst.Utilities;

namespace Amethyst.Networking.Packets.Login;

internal sealed class LoginSuccessPacket : IOutgoingPacket
{
    public int Identifier => 0x02;

    public required Guid Guid { get; init; }

    public required string Username { get; init; }

    private string? serializedGuid;

    public int CalculateLength()
    {
        serializedGuid = Guid.ToString();
        return VariableString.GetBytesCount(serializedGuid) + VariableString.GetBytesCount(Username);
    }

    public int Write(ref MemoryWriter writer)
    {
        writer.WriteVariableString(serializedGuid!);
        writer.WriteVariableString(Username);
        return writer.Position;
    }
}