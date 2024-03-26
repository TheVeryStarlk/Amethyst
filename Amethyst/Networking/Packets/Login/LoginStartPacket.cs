using Amethyst.Api.Components;
using Amethyst.Api.Events.Minecraft.Player;
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
        client.Transport.Queue(
            new LoginSuccessPacket
            {
                Guid = client.Player!.Guid,
                Username = client.Player.Username
            });

        var world = client.Server.Level!.Worlds.FirstOrDefault().Value;
        client.Player.World = world;

        client.Transport.Queue(
            new JoinGamePacket
            {
                Player = client.Player
            });

        client.Transport.Queue(
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

        client.Server.BroadcastPacket(
            new PlayerListItemPacket
            {
                Action = new AddPlayerAction(),
                Players = client.Server.Players
            });

        await world.SpawnPlayerAsync(client.Player);

        for (var x = -1; x < 1; x++)
        {
            for (var z = -1; z < 1; z++)
            {
                client.Transport.Queue(
                    new ChunkPacket
                    {
                        Chunk = world.GetChunk(new Position(x, 0, z))
                    }
                );
            }
        }
    }
}