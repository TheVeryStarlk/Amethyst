using Amethyst.Abstractions.Hosting;

namespace Amethyst.Extensions.Commands;

public static class AmethystBuilderExtensions
{
    public static IAmethystBuilder AddCommandsSubscriber(this IAmethystBuilder builder)
    {
        builder.AddSubscriber<CommandsSubscriber>();
        return builder;
    }
}