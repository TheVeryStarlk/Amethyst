using Amethyst.Api.Components;
using Amethyst.Extensions;

namespace Amethyst.Networking.Packets.Status;

internal sealed class StatusResponsePacket : IOutgoingPacket
{
    public int Identifier => 0x00;

    public required ServerStatus Status { get; init; }

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteVariableString(Status.Serialize());
    }
}