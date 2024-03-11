using Amethyst.Api;
using Amethyst.Api.Components;
using Amethyst.Api.Entities;
using Amethyst.Networking;
using Amethyst.Networking.Packets.Login;
using Amethyst.Networking.Packets.Playing;

namespace Amethyst.Entities;

internal sealed class Player(MinecraftClient client, string username) : IPlayer
{
    public Guid Guid { get; } = Guid.NewGuid();

    public int Identifier { get; } = Random.Shared.Next();

    public IMinecraftServer Server => client.Server;

    public string Username => username;

    public Position Position { get; set; }

    public float Yaw { get; set; }

    public float Pitch { get; set; }

    public bool OnGround { get; set; }

    public GameMode GameMode { get; set; }

    public async Task TeleportAsync(Position position)
    {
        Position = position;

        await client.Transport.Output.WritePacketAsync(
            new PlayerPositionAndLookPacket
            {
                Position = position,
                Yaw = Yaw,
                Pitch = Pitch,
                OnGround = OnGround
            });
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
            new DisconnectPacket(MinecraftClientState.Playing)
            {
                Reason = reason
            });
    }
}