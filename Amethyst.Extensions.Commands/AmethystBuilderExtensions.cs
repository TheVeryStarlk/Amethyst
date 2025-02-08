using Amethyst.Hosting;

namespace Amethyst.Extensions.Commands;

public static class AmethystBuilderExtensions
{
    public static AmethystOptions AddCommandsSubscriber(this AmethystOptions options)
    {
        options.AddSubscriber<CommandsSubscriber>();
        return options;
    }
}