using System.Net;
using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Eventing.Client;
using Amethyst.Abstractions.Eventing.Player;
using Amethyst.Abstractions.Eventing.Server;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Abstractions.Worlds;
using Amethyst.Example.Time;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Amethyst.Example.Subscribers;

/// <summary>
/// Presents the server in the server list, and welcomes players into the overworld.
/// </summary>
internal sealed class ServerSubscriber(ILogger<ServerSubscriber> logger, IConfiguration configuration, Realm realm, Clock clock, IWorldFactory worldFactory) : ISubscriber
{
    public void Subscribe(IRegistry registry)
    {
        registry.For<IServer>(consumer =>
        {
            consumer.On<Starting>((_, starting) =>
            {
                // Pass "--port 25566" to listen elsewhere.
                starting.EndPoint = new IPEndPoint(IPAddress.Any, configuration.GetValue("Port", 25565));
                logger.LogInformation("Listening on {EndPoint}", starting.EndPoint);
            });

            consumer.On<Stopping>((_, stopping) => stopping.Message = Message.Simple("The expedition is over, see you next time!"));
        });

        registry.For<IClient>(consumer =>
        {
            consumer.On<Request>((_, request) =>
            {
                request.Name = "Amethyst 1.8.9";
                request.Online = worldFactory.Players().Count();
                request.Favicon = realm.Favicon;
                request.Description = Message
                    .Create()
                    .Write("Amethyst ").LightPurple().Bold()
                    .Write("Expedition").White().Bold()
                    .WriteLine()
                    .Write($"Explore seed {realm.Seed}, wands included.").Gray()
                    .Build();
            });

            consumer.On<Outdated>((_, outdated) => outdated.Message = Message.Simple("Please join using Minecraft 1.8.9."));

            consumer.On<Joining>((_, joining) =>
            {
                joining.World = realm.World;
                joining.GameMode = GameMode.Creative;
            });
        });

        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Joined>((player, _) =>
            {
                // Players are placed at the origin on join, move them to the spawn.
                player.Teleport(realm.Spawn);

                // The clock service sends the time every second, but the sky should not flash in the meantime.
                player.Client.Write(new TimePacket(clock.Age, clock.Time));

                if (clock.Raining)
                {
                    player.Client.Write(new ChangeStatePacket(new StartRainingState()));
                }

                Kit.Give(player);

                logger.LogInformation("{Username} joined", player.Username);
            });

            consumer.On<Left>((player, _) => logger.LogInformation("{Username} left", player.Username));

            // Keep the bottom of the world intact so nobody falls into the void.
            consumer.On<Dig>((_, dig) => dig.IsCancelled = dig.Position.Y < 1);
        });
    }
}
