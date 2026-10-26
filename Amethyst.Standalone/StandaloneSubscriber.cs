using System.Net;
using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Eventing.Client;
using Amethyst.Abstractions.Eventing.Player;
using Amethyst.Abstractions.Eventing.Server;
using Amethyst.Abstractions.Worlds;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Amethyst.Standalone;

/// <summary>
/// Listens on the configured port and places every player in a single flat world.
/// </summary>
internal sealed class StandaloneSubscriber(ILogger<StandaloneSubscriber> logger, IConfiguration configuration, IWorldFactory worldFactory) : ISubscriber
{
    private IWorld? world;

    public void Subscribe(IRegistry registry)
    {
        registry.For<IServer>(consumer => consumer.On<Starting>((_, starting) =>
        {
            // Pass "--port 25566" or set the "Port" environment variable to listen elsewhere.
            starting.EndPoint = new IPEndPoint(IPAddress.Any, configuration.GetValue("Port", 25565));
            logger.LogInformation("Listening on {EndPoint}", starting.EndPoint);

            // Create the world once before anyone can join, players are tracked by the world they are in.
            world = worldFactory.Create("Flat", FlatGenerator.Instance);
        }));

        registry.For<IClient>(consumer => consumer.On<Joining>((_, joining) =>
        {
            joining.World = world;
            joining.GameMode = GameMode.Creative;
        }));

        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Joined>((player, _) =>
            {
                // Players are placed at the origin on join, move them on top of the grass.
                player.Teleport(new Position(0.5, FlatGenerator.Height, 0.5));
                logger.LogInformation("{Username} joined", player.Username);
            });

            consumer.On<Left>((player, _) => logger.LogInformation("{Username} left", player.Username));

            // Keep the bedrock intact so nobody falls into the void.
            consumer.On<Dig>((_, dig) => dig.IsCancelled = dig.Position.Y < 1);
        });
    }
}