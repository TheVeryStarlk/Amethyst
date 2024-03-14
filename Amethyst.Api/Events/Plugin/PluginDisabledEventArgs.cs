namespace Amethyst.Api.Events.Plugin;

public sealed class PluginDisabledEventArgs : AmethystEventArgsBase
{
    public required DateTimeOffset DateTimeOffset { get; init; }
}