using System.Collections.Frozen;

namespace Amethyst.Extensions.Commands;

public sealed class CommandsEngineBuilder
{
    private readonly Dictionary<string, CommandDelegate> callbacks = [];

    private CommandDelegate? failure;

    internal CommandsEngineBuilder()
    {
        // Not to be used publicly.
    }

    public CommandsEngineBuilder Map(string name, CommandDelegate callback)
    {
        if (!callbacks.TryAdd(name, callback))
        {
            throw new InvalidOperationException("Command already registered.");
        }

        return this;
    }

    public CommandsEngineBuilder Failure(CommandDelegate callback)
    {
        failure = callback;
        return this;
    }

    public CommandsEngine Build()
    {
        return new CommandsEngine(callbacks.ToFrozenDictionary(), failure);
    }
}