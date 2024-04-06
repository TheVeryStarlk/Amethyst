﻿using System.Text.Json;
using Amethyst.Api;
using Amethyst.Api.Components;
using Amethyst.Api.Entities;
using Amethyst.Api.Plugins.Events;
using Amethyst.Components;
using Amethyst.Extensions;

namespace Amethyst.Protocol.Packets.Playing;

internal sealed class ChatPacket : IIngoingPacket<ChatPacket>, IOutgoingPacket
{
    static int IIngoingPacket<ChatPacket>.Identifier => 0x01;

    public int Identifier => 0x02;

    public required Chat Chat { get; init; }

    public ChatPosition Position { get; init; }

    public static ChatPacket Read(MemoryReader reader)
    {
        return new ChatPacket
        {
            Chat = Chat.Create(reader.ReadVariableString())
        };
    }

    public async Task HandleAsync(IServer server, IPlayer player, IClient client)
    {
        if (Chat.Text.StartsWith('/'))
        {
            await server.PluginService.CommandService.ExecuteAsync(player, Chat.Text);
            return;
        }

        var @event = await server.PluginService.EventService.ExecuteAsync(
            new ChatSentEvent
            {
                Server = server,
                Chat = Chat
            });

        if (@event.IsHandled)
        {
            return;
        }

        await server.Players
            .Select(other => other.SendChatAsync($"{player.Username}: {Chat.Text}", Position))
            .WhenEach();
    }

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteVariableString(JsonSerializer.Serialize(Chat, MinecraftSerializerOptions.Instance));
        writer.WriteByte((byte) Position);
    }
}