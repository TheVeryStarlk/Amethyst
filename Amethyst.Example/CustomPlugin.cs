using Amethyst.Api.Components;
using Amethyst.Api.Plugin;
using Amethyst.Api.Plugin.Events;

namespace Amethyst.Example;

public sealed class CustomPlugin : PluginBase
{
    public override PluginConfiguration Configuration => new PluginConfiguration
    {
        Name = "Custom provider"
    };

    public override void ConfigureRegistry(IPluginRegistry registry)
    {
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