using Amethyst.Abstractions.Entities;

namespace Amethyst.Example.Creatures.Behaviors;

/// <summary>
/// Runs a rainbow through an entity's name, letter by letter.
/// </summary>
internal sealed class Shimmer(ISpawnable entity, string text, int pace = 3) : IBehavior
{
    // Red, gold, yellow, green, aqua, blue and light purple formatting codes.
    private const string Colors = "c6eab9d";

    private long age;

    public bool Step()
    {
        if (!entity.Tracked())
        {
            return false;
        }

        if (++age % pace != 0)
        {
            return true;
        }

        var shift = age / pace;
        var builder = new System.Text.StringBuilder(text.Length * 5);

        for (var index = 0; index < text.Length; index++)
        {
            builder.Append('§').Append(Colors[(int) ((index + shift) % Colors.Length)]).Append("§l").Append(text[index]);
        }

        entity.CustomName = builder.ToString();
        entity.Refresh();

        return true;
    }
}
