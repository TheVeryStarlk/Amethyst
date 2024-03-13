using Amethyst.Api;
using Amethyst.Api.Components;
using Amethyst.Api.Entities;
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

    public Position Position { get; set; }

    public float Yaw { get; set; }

    public float Pitch { get; set; }

    public bool OnGround { get; set; }

    public async Task TeleportAsync(Position position)
    {
        await client.Transport.Output.WritePacketAsync(
            new PlayerPositionAndLookPacket
            {
                Position = position,
                Yaw = Yaw,
                Pitch = Pitch,
                OnGround = OnGround
            });

        Position = position;
    }

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
        await client.Transport.Output.WritePacketAsync(
            new DisconnectPacket(ClientState.Playing)
            {
                Reason = reason
            });

        await client.StopAsync();
    }
}