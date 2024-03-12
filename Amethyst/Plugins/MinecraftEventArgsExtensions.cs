using Amethyst.Api.Plugins.Events;

namespace Amethyst.Plugins;

internal static class MinecraftEventArgsExtensions
{
    public static async Task<T> ExecuteAsync<T>(this PluginService pluginService, T eventArgs) where T : ServerEventArgsBase
    {
        await pluginService.EventService.ExecuteEventAsync(eventArgs);
        return eventArgs;
    }
}