using System.Diagnostics;

namespace Amethyst;

internal sealed class BalancingTimer(TimeSpan timeSpan)
{
    private readonly long ticks = timeSpan.Milliseconds * Stopwatch.Frequency / 1000L;
    private readonly Stopwatch stopwatch = new();

    private long delay;

    public async ValueTask<bool> WaitForNextTickAsync(CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return false;
        }

        var delta = stopwatch.ElapsedTicks;
        stopwatch.Restart();

        delay += delta - ticks;

        if (delay >= 0)
        {
            return true;
        }

        var extraTimeInMilliseconds = (int) (-delay * 1000L / Stopwatch.Frequency);
        await Task.Delay(extraTimeInMilliseconds, cancellationToken).ConfigureAwait(false);

        return true;
    }
}