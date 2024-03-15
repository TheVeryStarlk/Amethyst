using Amethyst.Api.Components;
using Amethyst.Extensions;
using Amethyst.Utilities;

namespace Amethyst.Networking.Packets.Status;

internal sealed class StatusResponsePacket : IOutgoingPacket
{
    public int Identifier => 0x00;

    public required ServerStatus Status { get; init; }

    private string? serializedStatus;

    public int CalculateLength()
    {
        serializedStatus = Status.Serialize();
        return VariableString.GetBytesCount(serializedStatus);
    }

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteVariableString(serializedStatus!);
    }
}