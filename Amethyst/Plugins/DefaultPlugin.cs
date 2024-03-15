using System.Text;
using Amethyst.Api.Components;
using Amethyst.Api.Plugins;

namespace Amethyst.Plugins;

internal sealed class DefaultPlugin : PluginBase
{
    public override PluginConfiguration Configuration => new PluginConfiguration
    {
        Name = "Default plugin",
        Description = "Provides common functionalities for the server."
    };

    public override void ConfigureRegistry(IPluginRegistry pluginRegistry)
    {
        pluginRegistry.RegisterCommand(
            "plugins",
            async command =>
            {
                if (command.Arguments.Length > 0)
                {
                    await command.Player.SendChatMessageAsync(ChatMessage.Create("Invalid usage.", Color.Red));
                    return;
                }

                var plugins = ((Server) command.Server).PluginService.Plugins.Keys.ToArray();

                var message = ChatMessage.Create(
                    $"Plugins ({plugins.Length}): {string.Join(", ", plugins)}.",
                    Color.Green);

                await command.Player.SendChatMessageAsync(message);
            });
    }
}