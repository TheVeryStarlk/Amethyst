using Amethyst.Api.Components;
using Amethyst.Api.Plugins;
using Amethyst.Api.Plugins.Events;

namespace Amethyst.Example;

public sealed class CustomPlugin : PluginBase
{
    public override PluginConfiguration Configuration => new PluginConfiguration
    {
        Name = "Custom provider"
    };

    public override void ConfigureRegistry(IPluginRegistry registry)
    {
        registry.RegisterCommand(
            "kick",
            async eventArgs =>
            {
                if (eventArgs.Arguments.Length == 2)
                {
                    await eventArgs.Player.Server.DisconnectPlayerAsync(
                        eventArgs.Player.Server.Players.First(player =>
                            player.Username.Equals(eventArgs.Arguments[0], StringComparison.CurrentCultureIgnoreCase)),
                        ChatMessage.Create(eventArgs.Arguments[1], Color.Red));
                }
                else
                {
                    await eventArgs.Player.SendChatMessageAsync(
                        ChatMessage.Create("Invalid kick command.", Color.Red));
                }
            });

        registry.RegisterEvent<DescriptionRequestedEventArgs>(eventArgs =>
        {
            eventArgs.Description = ChatMessage.Create($"Current date is {DateTime.Now}");
            return Task.CompletedTask;
        });

        registry.RegisterEvent<PlayerJoinedEventArgs>(eventArgs =>
        {
            eventArgs.Message = ChatMessage.Create($"Welcome {eventArgs.Player.Username}!", Color.Green);
            return Task.CompletedTask;
        });

        registry.RegisterEvent<PlayerLeaveEventArgs>(eventArgs =>
        {
            eventArgs.Message = ChatMessage.Create($"Good bye {eventArgs.Player.Username}...", Color.Red);
            return Task.CompletedTask;
        });
    }
}