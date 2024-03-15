using Amethyst.Api.Components;
using Amethyst.Api.Events.Minecraft.Player;
using Amethyst.Extensions;
using Amethyst.Utilities;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class ChatMessagePacket : IIngoingPacket<ChatMessagePacket>, IOutgoingPacket
{
    static int IIngoingPacket<ChatMessagePacket>.Identifier => 0x01;

    public int Identifier => 0x02;

    public required ChatMessage Message { get; init; }

    public ChatMessagePosition Position { get; init; }

    private string? serializedMessage;

    public static ChatMessagePacket Read(MemoryReader reader)
    {
        return new ChatMessagePacket
        {
            Message = ChatMessage.Create(reader.ReadVariableString()),
            Position = ChatMessagePosition.Box,
        };
    }

    public int CalculateLength()
    {
        serializedMessage = Message.Serialize();
        return VariableString.GetBytesCount(serializedMessage) + sizeof(byte);
    }

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteVariableString(serializedMessage!);
        writer.WriteByte((byte) Position);
    }

    public async Task HandleAsync(Client client)
    {
        if (Message.Text.StartsWith('/'))
        {
            await client.Server.CommandService.ExecuteAsync(
                client.Player!,
                Message.Text[1..]);
        }
        else
        {
            var eventArgs = await client.Server.EventService.ExecuteAsync(
                new ChatMessageSentEventArgs
                {
                    Server = client.Server,
                    Player = client.Player!,
                    Message = $"{client.Player!.Username}: {Message.Text}"
                });

            await client.Server.BroadcastChatMessageAsync(eventArgs.Message);
        }
    }
}