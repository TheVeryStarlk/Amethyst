using System.Collections.Frozen;
using Amethyst.Components.Entities;

namespace Amethyst.Extensions.Commands;

public sealed class CommandsEngine
{
    public string[] Registered { get; }

    private readonly FrozenDictionary<string, CommandDelegate> callbacks;
    private readonly CommandDelegate failure;

    internal CommandsEngine(FrozenDictionary<string, CommandDelegate> callbacks, CommandDelegate? failure)
    {
        this.callbacks = callbacks;
        this.failure = failure ?? ((player, _) => player.Send("Failed to execute command."));

        Registered = callbacks.Keys.ToArray();
    }

    public static CommandsEngineBuilder Create()
    {
        return new CommandsEngineBuilder();
    }

    public bool TryExecute(IPlayer player, string message)
    {
        if (message.Length is 0 || message[0] is not '/')
        {
            return false;
        }

        var parts = message[1..].Split();

        if (callbacks.TryGetValue(parts[0], out var callback))
        {
            callback(player, parts.Length > 0 ? parts[1..] : []);
        }
        else
        {
            failure(player, parts);
        }

        return true;
    }
}