using Microsoft.Extensions.Hosting;

namespace Amethyst.Example.Creatures;

/// <summary>
/// Steps the <see cref="Animator"/> at the game's twenty ticks a second.
/// </summary>
internal sealed class AnimatorService(Animator animator) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(50));

        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
            {
                animator.Advance();
            }
        }
        catch (OperationCanceledException)
        {
            // The host is stopping.
        }
    }
}
