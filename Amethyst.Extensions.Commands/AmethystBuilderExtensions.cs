using Amethyst.Abstractions.Hosting;

namespace Amethyst.Extensions.Commands;

public static class AmethystBuilderExtensions
{
    public static IAmethystBuilder AddCommands(this IAmethystBuilder builder)
    {
        return builder;
    }
}