namespace Amethyst;

internal static class TaskExtensions
{
    public static async Task TimeoutAfter(this Task task, TimeSpan timeout)
    {
        using var source = new CancellationTokenSource();

        var delayTask = Task.Delay(timeout, source.Token);
        var resultTask = await Task.WhenAny(task, delayTask).ConfigureAwait(false);

        if (resultTask == delayTask)
        {
            return;
        }

        await source.CancelAsync().ConfigureAwait(false);
        await task.ConfigureAwait(false);
    }
}