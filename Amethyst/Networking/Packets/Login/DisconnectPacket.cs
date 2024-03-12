using Amethyst.Api.Components;
using Amethyst.Extensions;
using Amethyst.Utilities;

namespace Amethyst.Networking.Packets.Login;

internal sealed class DisconnectPacket(ClientState state) : IOutgoingPacket
{
    public int Identifier => state is ClientState.Login ? 0x00 : 0x40;

    public required ChatMessage Reason { get; init; }

    private string? serializedReason;

    public int CalculateLength()
    {
        serializedReason = Reason.Serialize();
        return VariableString.GetBytesCount(serializedReason);
    }

    public int Write(ref MemoryWriter writer)
    {
        writer.WriteVariableString(serializedReason!);
        return writer.Position;
    }
}