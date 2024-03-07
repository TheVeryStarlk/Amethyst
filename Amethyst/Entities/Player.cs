﻿using System.Numerics;
using Amethyst.Api;
using Amethyst.Api.Components;
using Amethyst.Api.Entities;
using Amethyst.Api.Plugins.Events;
using Amethyst.Networking;
using Amethyst.Networking.Packets.Playing;
using Amethyst.Plugins;

namespace Amethyst.Entities;

internal sealed class Player(MinecraftClient client, string username) : IPlayer
{
    public Guid Guid { get; } = Guid.NewGuid();

    public int Identifier { get; } = Random.Shared.Next();

    public IMinecraftServer Server => client.Server;

    public string Username => username;

    public Vector3 Position { get; set; }

    public Vector2 Rotation { get; set; }

    public GameMode GameMode { get; set; }

    public async Task SendChatMessageAsync(ChatMessage message, ChatMessagePosition position = ChatMessagePosition.Box)
    {
        await client.Transport.Output.WritePacketAsync(
            new ChatMessagePacket
            {
                Message = message,
                Position = position
            });
    }

    public async Task DisconnectAsync(ChatMessage reason)
    {
        Server.Status.PlayerInformation.Online--;

        await client.Transport.Output.WritePacketAsync(
            new DisconnectPacket(MinecraftClientState.Playing)
            {
                Reason = reason
            });

        var eventArgs = await client.Server.PluginService.ExecuteAsync(
            new PlayerLeaveEventArgs
            {
                Server = client.Server,
                Player = this,
                Message = ChatMessage.Create($"{Username} has left the server.", Color.Yellow)
            });

        await client.StopAsync();
        await Server.BroadcastChatMessageAsync(eventArgs.Message);
    }
}