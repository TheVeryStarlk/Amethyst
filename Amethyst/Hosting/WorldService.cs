using Amethyst.Api.Plugins.Events.Server;
using Amethyst.Api.Worlds;
using Amethyst.Components;
using Amethyst.Extensions;
using Amethyst.Plugins;
using Amethyst.Worlds;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Amethyst.Hosting;

internal sealed class WorldService(
    ILogger<WorldService> logger,
    EventDispatcher eventDispatcher,
    Server server) : BackgroundService
{
    private CancellationTokenSource? source;

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        source = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        var worlds = new World[]
        {
            new World
            {
                Server = server,
                Type = WorldType.Flat,
                Difficulty = Difficulty.Peaceful,
                Dimension = Dimension.OverWorld
            }
        };

        server.Worlds = new Dictionary<string, IWorld>()
        {
            {
                "Flat",
                worlds[0]
            }
        };

        eventDispatcher.Register<ServerStartingEvent>(_ =>
        {
            source.Cancel();
            return Task.CompletedTask;
        });

        try
        {
            await Task.Delay(-1, source.Token);
        }
        catch
        {
            // Wait till the server starts.
        }

        logger.LogDebug("Started ticking worlds");

        var timer = new BalancingTimer(50, source.Token);

        while (await timer.WaitForNextTickAsync())
        {
            try
            {
                await worlds
                    .Select(world => world.TickAsync())
                    .WhenEach();
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(
                    "Unexpected exception while ticking worlds: \"{Exception}\"",
                    exception);

                break;
            }
        }
    }
}