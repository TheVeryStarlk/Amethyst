﻿using Amethyst.Api;
using Amethyst.Api.Components;
using Amethyst.Api.Plugins;
using Amethyst.Api.Plugins.Events.Server;
using Microsoft.Extensions.Logging;

namespace Amethyst.Plugin;

/// <summary>
/// A Minecraft <see cref="IServer"/> plugin that has a number of functionalities.
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

    /// <summary>
    /// Registers to all events and handles them.
    /// </summary>
    /// <param name="pluginRegistry">The registrar to use.</param>
    public override void ConfigureRegistry(IPluginRegistry pluginRegistry)
    {
        pluginRegistry.RegisterEvent<ServerStartingEvent>(
            @event =>
            {
                Logger.LogInformation("Server began starting @ {DateTimeOffset}!", @event.DateTimeOffset);
                return Task.CompletedTask;
            });

        pluginRegistry.RegisterCommand(
            "broadcast",
            async command =>
            {
                if (command.Arguments.Length == 0)
                {
                    var error = Chat.Create("Specify a message.", Color.Red);
                    await command.Player.SendChatAsync(error);
                    return;
                }

                foreach (var player in command.Server.Players)
                {
                    await player.SendChatAsync(command.Arguments[0]);
                }
            });
    }
}