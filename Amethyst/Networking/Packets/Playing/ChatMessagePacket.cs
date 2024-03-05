using Amethyst.Api.Components;
using Amethyst.Networking.Packets.Status;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class ChatMessagePacket : IIngoingPacket<ChatMessagePacket>, IOutgoingPacket
{
    static int IIngoingPacket<ChatMessagePacket>.Identifier => 0x01;

    public int Identifier => 0x02;

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

    public static ChatMessagePacket Read(MemoryReader reader)
    {
        return new ChatMessagePacket
        {
            Message = ChatMessage.Create(reader.ReadVariableString()),
            Position = ChatMessagePosition.Box,
        };
    }

    public async Task HandleAsync(MinecraftClient client)
    {
        await client.Server.BroadcastChatMessage(ChatMessage.Create($"{client.Player!.Username}: {Message.Text}"));
    }
}