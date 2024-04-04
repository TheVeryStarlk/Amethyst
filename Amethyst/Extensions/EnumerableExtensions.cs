namespace Amethyst.Extensions;

internal static class EnumerableExtensions
{
    public static async Task WhenEach(this IEnumerable<Task> tasks)
    {
        await Task.WhenAll(tasks);
    }
}