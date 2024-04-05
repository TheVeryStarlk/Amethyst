using Amethyst.Api;
using Amethyst.Api.Plugins;

namespace Amethyst.Plugin;

/// <summary>
/// A Minecraft <see cref="IServer"/> plugin that has a number of features.
/// </summary>
internal sealed class DefaultPlugin : PluginBase
{
    /// <summary>
    /// Holds useful information about the plugin.
    /// </summary>
    public override PluginInformation Information => new PluginInformation
    {
        Name = "Default Plugin",
        Description = "A plugin that shows Amethyst's API."
    };
}