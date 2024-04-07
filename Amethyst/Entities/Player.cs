﻿using Amethyst.Api.Components;
using Amethyst.Api.Entities;
using Amethyst.Protocol;
using Amethyst.Protocol.Packets.Playing;

namespace Amethyst.Entities;

internal sealed class Player(IClient client) : EntityBase, IPlayer
{
    public required Guid Guid { get; init; }

    public required string Username { get; init; }

    public GameMode GameMode { get; set; }

    public byte ViewDistance
    {
        get => viewDistance;
        set => viewDistance = value > Server.Options.ViewDistance
            ? Server.Options.ViewDistance
            : value;
    }

    private byte viewDistance;

    public void Join()
    {
        IOutgoingPacket[] packets =
        [
            new JoinGamePacket
            {
                Player = this
            },
            new PlayerPositionAndLookPacket
            {
                Vector = Vector,
                Yaw = Yaw,
                Pitch = Pitch,
                OnGround = OnGround
            }
        ];

        foreach (var packet in packets)
        {
            client.Queue(packet);
        }
    }

    public void SendChat(Chat chat, ChatPosition position)
    {
        client.Queue(
            new ChatPacket
            {
                Chat = chat,
                Position = position
            });
    }

    public void Kick(Chat reason)
    {
        client.Stop();
    }
}