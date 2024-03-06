﻿using System.Numerics;
using Amethyst.Api;
using Amethyst.Api.Components;
using Amethyst.Api.Entities;
using Amethyst.Api.Plugin.Events;
using Amethyst.Networking;
using Amethyst.Networking.Packets.Playing;

namespace Amethyst.Entities;

internal sealed class Player(MinecraftClient client, string username) : IPlayer
{
    public string Username => username;

    public int Identifier { get; } = Random.Shared.Next();

    public Guid Guid { get; } = Guid.NewGuid();

    public IMinecraftServer Server => client.Server;

    public Vector3 Position { get; set; }

    public Vector2 Rotation { get; set; }

    public GameMode GameMode { get; set; }

    public async Task JoinAsync()
    {
        await client.Transport.Output.WritePacketAsync(
            new JoinGamePacket
            {
                Player = this
            },
            client.CancellationToken);

        await client.Transport.Output.WritePacketAsync(
            new PlayerPositionAndLookPacket
            {
                Player = this
            },
            client.CancellationToken);

        var eventArgs = new PlayerJoinedEventArgs
        {
            Server = client.Server,
            Player = this,
            Message = ChatMessage.Create($"{Username} has joined the server", Color.Yellow)
        };

        await Server.BroadcastChatMessage(eventArgs.Message);
        Server.Status.PlayerInformation.Online++;
    }

    public async Task SendChatMessageAsync(ChatMessage message, ChatMessagePosition position = ChatMessagePosition.Box)
    {
        await client.Transport.Output.WritePacketAsync(
            new ChatMessagePacket
            {
                Message = message,
                Position = position
            },
            client.CancellationToken);
    }

    public async Task KickAsync(ChatMessage reason)
    {
        await client.Transport.Output.WritePacketAsync(
            new DisconnectPacket(MinecraftClientState.Playing)
            {
                Reason = reason
            },
            client.CancellationToken);

        await DisconnectAsync();
    }

    public async Task DisconnectAsync()
    {
        var eventArgs = new PlayerLeaveEventArgs
        {
            Server = client.Server,
            Player = this,
            Message = ChatMessage.Create($"{Username} has left the server", Color.Yellow)
        };

        await Server.BroadcastChatMessage(eventArgs.Message);
        Server.Status.PlayerInformation.Online--;
        await client.StopAsync();
    }
}