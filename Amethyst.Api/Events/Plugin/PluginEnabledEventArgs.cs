namespace Amethyst.Api.Events.Plugin;

public sealed class PluginEnabledEventArgs : AmethystEventArgsBase
{
    public required DateTimeOffset DateTimeOffset { get; init; }
}