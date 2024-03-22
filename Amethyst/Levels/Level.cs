using Amethyst.Api.Levels;

namespace Amethyst.Levels;

internal sealed class Level : ILevel
{
    public Dictionary<string, IWorld> Worlds { get; } = [];

    public async Task TickAsync(CancellationToken cancellationToken)
    {
        foreach (var world in Worlds.Values)
        {
            await world.TickAsync();
        }
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }
}