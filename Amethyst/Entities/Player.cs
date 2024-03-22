using Amethyst.Api;
using Amethyst.Api.Components;
using Amethyst.Api.Entities;
using Amethyst.Api.Levels;
using Amethyst.Extensions;
using Amethyst.Networking.Packets.Login;
using Amethyst.Networking.Packets.Playing;

namespace Amethyst.Entities;

internal sealed class Player(Client client, string username) : IPlayer
{
    public Guid Guid { get; } = Guid.NewGuid();

    public IServer Server => client.Server;

    public string Username => username;

    public GameMode GameMode { get; set; }

    public int Identifier { get; } = Random.Shared.Next();

    public IWorld? World { get; set; }

    public VectorF Position
    {
        get => position;
        set
        {
            OldPosition = Position;
            position = value;
        }
    }

    public VectorF OldPosition { get; set; }

    public float Yaw { get; set; }

    public float Pitch { get; set; }

    public bool OnGround { get; set; }

    private VectorF position;

    public Task TeleportAsync(VectorF destination)
    {
        client.Transport.Output.QueuePacket(
            new PlayerPositionAndLookPacket
            {
                Position = destination,
                Yaw = Yaw,
                Pitch = Pitch,
                OnGround = OnGround
            });

        Position = destination;
        return Task.CompletedTask;
    }

    public Task SendChatMessageAsync(ChatMessage message, ChatMessagePosition messagePosition = ChatMessagePosition.Box)
    {
        client.Transport.Output.QueuePacket(
            new ChatMessagePacket
            {
                Message = message,
                Position = messagePosition
            });

        return Task.CompletedTask;
    }

    public async Task DisconnectAsync(ChatMessage reason)
    {
        await client.Transport.Output.WritePacketAsync(
            new DisconnectPacket(ClientState.Playing)
            {
                Reason = reason
            });

        await client.StopAsync();
    }

    public Task SpawnPlayerAsync(IPlayer player)
    {
        client.Transport.Output.QueuePacket(
            new SpawnPlayerPacket
            {
                Player = player
            });

        return Task.CompletedTask;
    }

    public Task DestroyEntitiesAsync(params IEntity[] entities)
    {
        client.Transport.Output.QueuePacket(
            new DestroyEntitiesPacket
            {
                Entities = entities
            });

        return Task.CompletedTask;
    }

    public Task UpdateEntitiesAsync(params IEntity[] entities)
    {
        foreach (var entity in entities)
        {
            client.Transport.Output.QueuePacket(
                new EntityLookAndRelativeMovePacket
                {
                    Entity = entity
                });
        }

        return Task.CompletedTask;
    }
}