using Amethyst.Api.Components;
using Amethyst.Api.Plugins.Events;
using Amethyst.Networking.Packets.Playing;
using Amethyst.Plugins;

namespace Amethyst.Networking.Packets.Login;

internal sealed class LoginStartPacket : IIngoingPacket<LoginStartPacket>
{
    public static int Identifier => 0x00;

    public required string Username { get; init; }

    public static LoginStartPacket Read(MemoryReader reader)
    {
        return new LoginStartPacket
        {
            Username = reader.ReadVariableString()
        };
    }

    public async Task HandleAsync(MinecraftClient client)
    {
        client.Server.Status.PlayerInformation.Online++;

        await client.Transport.Output.WritePacketAsync(
            new LoginSuccessPacket
            {
                Guid = client.Player!.Guid,
                Username = client.Player.Username
            });

        await client.Transport.Output.WritePacketAsync(
            new JoinGamePacket
            {
                Player = client.Player
            });

        await client.Transport.Output.WritePacketAsync(
            new PlayerPositionAndLookPacket
            {
                Position = client.Player.Position,
                Rotation = client.Player.Rotation
            });

        var eventArgs = await client.Server.PluginService.ExecuteAsync(
            new PlayerJoinedEventArgs
            {
                Server = client.Server,
                Player = client.Player,
                Message = ChatMessage.Create($"{client.Player.Username} has joined the server.", Color.Yellow)
            });

        await client.Server.BroadcastChatMessageAsync(eventArgs.Message);
    }
}