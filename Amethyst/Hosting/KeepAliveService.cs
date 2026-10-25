using Amethyst.Abstractions.Packets.Play;
using Amethyst.Abstractions.Worlds;
using Microsoft.Extensions.Hosting;

namespace Amethyst.Hosting;

// Clients disconnect if they do not receive a keep alive in time.
internal sealed class KeepAliveService(IWorldFactory worldFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
            {
                var alive = new KeepAlivePacket(Environment.TickCount);

                // Every player in the play state is tracked by a world.
                foreach (var world in worldFactory.Worlds)
                {
                    foreach (var pair in world.Players)
                    {
                        pair.Value.Client.Write(alive);
                    }
                }
            }
        }
        catch (OperationCanceledException)
        {
            // The host is stopping.
        }
    }
}