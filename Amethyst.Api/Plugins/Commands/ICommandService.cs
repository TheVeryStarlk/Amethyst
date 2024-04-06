using Amethyst.Api.Entities;

namespace Amethyst.Api.Plugins.Commands;

public interface ICommandService
{
    public IEnumerable<string> Commands { get; }

    public Task ExecuteAsync(IPlayer player, string chat);
}

public sealed class CommandInformation
{
    public required IServer Server { get; init; }

    public required IPlayer Player { get; init; }

    public string[] Arguments { get; init; } = Array.Empty<string>();
}