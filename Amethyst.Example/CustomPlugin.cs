using Amethyst.Api.Events.Minecraft;
using Amethyst.Api.Events.Plugin;
using Amethyst.Api.Plugins;
using Microsoft.Extensions.Logging;

namespace Amethyst.Example;

public sealed class CustomPlugin : PluginBase
{
    public override PluginConfiguration Configuration => new PluginConfiguration
    {
        Name = "Custom plugin",
        Description = "A custom plugin that demonstrates the API."
    };

    public override void ConfigureRegistry(IPluginRegistry registry)
    {
        registry.RegisterEvent<DescriptionRequestedEventArgs>(
            async eventArgs =>
            {
                eventArgs.Description = "Hello from plugin!";
                await eventArgs.Server.BroadcastChatMessageAsync("Status requested!");
            });

        registry.RegisterEvent<PluginEnabledEventArgs>(
            eventArgs =>
            {
                Logger!.LogInformation("I have been enabled @ {Offset}", eventArgs.DateTimeOffset);
                return Task.CompletedTask;
            });

        registry.RegisterEvent<PluginDisabledEventArgs>(
            eventArgs =>
            {
                Logger!.LogWarning("I have been disabled @ {Offset}", eventArgs.DateTimeOffset);
                return Task.CompletedTask;
            });
    }
}