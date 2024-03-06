using Amethyst.Api.Plugin.Events;

namespace Amethyst.Plugin;

internal static class MinecraftEventArgsExtensions
{
    public static async Task<T> ExecuteAsync<T>(this PluginService pluginService, T eventArgs) where T : MinecraftEventArgsBase
    {
        await pluginService.ExecuteEventAsync(eventArgs);
        return eventArgs;
    }
}