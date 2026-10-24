using Amethyst.Abstractions.Packets.Play;
using Amethyst.Entities.Players;
using Microsoft.Extensions.Hosting;

namespace Amethyst.Hosting;

// Clients disconnect if they do not receive a keep alive in time.
internal sealed class KeepAliveService(PlayerRepository playerRepository) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));

        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
            {
                var alive = new KeepAlivePacket(Environment.TickCount);

                foreach (var pair in playerRepository.Players)
                {
                    pair.Value.Client.Write(alive);
                }
            }
        }
        catch (OperationCanceledException)
        {
            // The host is stopping.
        }
    }
}
