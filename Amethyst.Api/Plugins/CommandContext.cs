using Amethyst.Api.Entities;

namespace Amethyst.Api.Plugins;

public sealed class CommandContext
{
    public required IPlayer Player { get; init; }

    public required string Name { get; init; }

    public string[]? Arguments { get; init; }
}