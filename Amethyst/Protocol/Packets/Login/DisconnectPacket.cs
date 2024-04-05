using System.Text.Json;
using Amethyst.Api.Components;
using Amethyst.Components;

namespace Amethyst.Protocol.Packets.Login;

internal sealed class DisconnectPacket(bool isPlaying) : IOutgoingPacket
{
    public int Identifier => isPlaying ? 0x40 : 0x00;

    public required Chat Reason { get; init; }

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteVariableString(JsonSerializer.Serialize(Reason, MinecraftSerializerOptions.Instance));
    }
}