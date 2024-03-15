using Amethyst.Api.Components;
using Amethyst.Api.Events.Minecraft;
using Amethyst.Api.Events.Plugin;
using Amethyst.Api.Plugins;
using Microsoft.Extensions.Logging;

namespace Amethyst.Plugin;

public sealed class CustomPlugin : PluginBase
{
    public override PluginConfiguration Configuration => new PluginConfiguration
    {
        Name = "Custom plugin",
        Description = "A custom plugin that demonstrates the API."
    };

    public override void ConfigureRegistry(IPluginRegistry registry)
    {
        registry.RegisterCommand(
            "kick",
            async command =>
            {
                if (command.Arguments.Length >= 1)
                {
                    var player = command.Server.Players.FirstOrDefault(player => player.Username.Equals(
                        command.Arguments[0],
                        StringComparison.CurrentCultureIgnoreCase));

                    if (player is null)
                    {
                        await command.Player.SendChatMessageAsync(ChatMessage.Create(
                            $"Could not find player: \"{command.Arguments[0]}\".",
                            Color.Red));

                        return;
                    }

                    if (command.Arguments.Length == 1)
                    {
                        await command.Player.SendChatMessageAsync(ChatMessage.Create(
                            "Must provide a kick reason.",
                            Color.Red));

                        return;
                    }

                    await command.Server.DisconnectPlayerAsync(player, string.Join(" ", command.Arguments[1..]));
                }
                else
                {
                    await command.Player.SendChatMessageAsync(ChatMessage.Create(
                        "Invalid kick command.",
                        Color.Red));
                }
            });

        registry.RegisterEvent<DescriptionRequestedEventArgs>(
            async eventArgs =>
            {
                eventArgs.Description = "Hello from plugin!";
                await eventArgs.Server.BroadcastChatMessageAsync("Status requested!");
            });

        registry.RegisterEvent<PluginEnabledEventArgs>(
            eventArgs =>
            {
                Logger.LogInformation("I have been enabled @ {Offset}", eventArgs.DateTimeOffset);
                return Task.CompletedTask;
            });

        registry.RegisterEvent<PluginDisabledEventArgs>(
            eventArgs =>
            {
                Logger.LogInformation("I have been disabled @ {Offset}", eventArgs.DateTimeOffset);
                return Task.CompletedTask;
            });
    }
}