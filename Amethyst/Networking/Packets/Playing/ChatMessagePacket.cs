using Amethyst.Api.Components;
using Amethyst.Networking.Packets.Status;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class ChatMessagePacket : IOutgoingPacket
{
    public static int Identifier => 0x02;

    public required ChatMessage Message { get; init; }

    public ChatMessagePosition Position { get; init; }

    private string? serializedMessage;

    public int CalculateLength()
    {
        serializedMessage = Message.Serialize();
        return VariableStringHelper.GetBytesCount(serializedMessage) + sizeof(byte);
    }

    public int Write(ref MemoryWriter writer)
    {
        writer.WriteVariableString(serializedMessage!);
        writer.WriteByte((byte) Position);
        return writer.Position;
    }
}