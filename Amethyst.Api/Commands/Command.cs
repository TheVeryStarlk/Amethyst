using Amethyst.Api.Entities;

namespace Amethyst.Api.Commands;

public sealed class Command
{
    public required IServer Server { get; init; }

    public required IPlayer Player { get; init; }

    public string[] Arguments { get; init; } = Array.Empty<string>();
}