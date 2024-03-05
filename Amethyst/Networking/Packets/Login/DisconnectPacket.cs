using Amethyst.Api.Components;
using Amethyst.Networking.Packets.Status;

namespace Amethyst.Networking.Packets.Login;

internal sealed class DisconnectPacket: IOutgoingPacket
{
    public int Identifier => 0x00;

    public required ChatMessage Reason { get; init; }

    private string? serializedReason;

    public int CalculateLength()
    {
        serializedReason = Reason.Serialize();
        return VariableStringHelper.GetBytesCount(serializedReason);
    }

    public int Write(ref MemoryWriter writer)
    {
        writer.WriteVariableString(serializedReason!);
        return writer.Position;
    }
}