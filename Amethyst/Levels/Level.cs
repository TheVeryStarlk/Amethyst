using Amethyst.Api.Levels;

namespace Amethyst.Levels;

internal sealed class Level : ILevel
{
    public Dictionary<string, IWorld> Worlds { get; } = [];

    public Task TickAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}