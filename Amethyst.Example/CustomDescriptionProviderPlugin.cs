using Amethyst.Api.Components;
using Amethyst.Api.Plugin;
using Amethyst.Api.Plugin.Events;

namespace Amethyst.Example;

public sealed class CustomDescriptionProviderPlugin : PluginBase
{
    public override PluginConfiguration Configuration => new PluginConfiguration
    {
        Name = "Custom description provider"
    };

    public override void ConfigureRegistry(IPluginRegistry registry)
    {
        registry.RegisterEvent<DescriptionRequestedEventArgs>(eventArgs =>
        {
            eventArgs.Description = ChatMessage.Create($"Current date is {DateTime.Now}");
            return Task.CompletedTask;
        });
    }
}