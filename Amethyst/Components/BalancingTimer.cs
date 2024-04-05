using System.Diagnostics;

namespace Amethyst.Components;

// https://github.com/ObsidianMC/Obsidian/blob/master/Obsidian/Utilities/BalancingTimer.cs ♥️
internal sealed class BalancingTimer(int milliseconds, CancellationToken cancellationToken)
{
    private long delay;

    private readonly Stopwatch stopwatch = new Stopwatch();
    private readonly long ticksInterval = milliseconds * Stopwatch.Frequency / 1000L;

    public async ValueTask<bool> WaitForNextTickAsync()
    {
        if (cancellationToken.IsCancellationRequested)
        {
            return false;
        }

        var delta = stopwatch.ElapsedTicks;
        stopwatch.Restart();

        delay += delta - ticksInterval;

        if (delay >= 0)
        {
            return true;
        }

        var extraTimeInMilliseconds = (int)(-delay * 1000L / Stopwatch.Frequency);
        await Task.Delay(extraTimeInMilliseconds, cancellationToken);

        return true;
    }
}