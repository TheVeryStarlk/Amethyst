using Amethyst.Api.Components;
using Amethyst.Api.Events.Minecraft.Player;
using Amethyst.Extensions;
using Amethyst.Networking.Packets.Playing;

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

    public async Task HandleAsync(Client client)
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
                Yaw = client.Player.Yaw,
                Pitch = client.Player.Pitch,
            });

        var eventArgs = await client.Server.EventService.ExecuteAsync(
            new PlayerJoinedEventArgs
            {
                Server = client.Server,
                Player = client.Player,
                Message = ChatMessage.Create($"{client.Player.Username} has joined the server.", Color.Yellow)
            });

        await client.Server.BroadcastChatMessageAsync(eventArgs.Message);
    }
}