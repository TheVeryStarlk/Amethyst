using Amethyst.Api.Components;
using Amethyst.Networking.Packets;
using Amethyst.Networking.Packets.Status;

namespace Amethyst.Networking;

internal sealed class DisconnectPacket(MinecraftClientState state) : IOutgoingPacket
{
    public int Identifier => state is MinecraftClientState.Login ? 0x00 : 0x40;

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