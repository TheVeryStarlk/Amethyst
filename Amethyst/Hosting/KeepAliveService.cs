using Amethyst.Abstractions.Packets.Play;
using Amethyst.Abstractions.Worlds;
using Microsoft.Extensions.Hosting;

namespace Amethyst.Hosting;

// This is not a "complete" implementation.
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