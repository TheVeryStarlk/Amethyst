using Amethyst.Api.Levels;
using Amethyst.Api.Levels.Generators;
using Amethyst.Levels;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Amethyst.Services;

internal sealed class LevelService(ILogger<LevelService> logger, Server server) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        var world = new World(server, "Flat", new FlatWorldGenerator())
        {
            Type = WorldType.Flat,
            Difficulty = Difficulty.Peaceful,
            Dimension = Dimension.OverWorld
        };

        var level =  new Level
        {
            Worlds =
            {
                {
                    world.Name,
                    world
                }
            }
        };

        server.Level = level;

        logger.LogInformation("Started ticking level worlds");

        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(5));

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await timer.WaitForNextTickAsync(cancellationToken);
                await level.TickAsync(cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(
                    "Unexpected exception while ticking level worlds: \"{Exception}\"",
                    exception);

                break;
            }
            finally
            {
                await level.DisposeAsync();
            }
        }
    }
}