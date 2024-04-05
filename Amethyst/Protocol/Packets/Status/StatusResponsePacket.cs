using System.Text.Json;
using Amethyst.Components;

namespace Amethyst.Protocol.Packets.Status;

internal sealed class StatusResponsePacket : IOutgoingPacket
{
    public int Identifier => 0x00;

    public required ServerStatus Status { get; init; }

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteVariableString(JsonSerializer.Serialize(Status, MinecraftSerializerOptions.Instance));
    }
}