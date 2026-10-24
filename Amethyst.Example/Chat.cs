using Amethyst.Abstractions.Messages;

namespace Amethyst.Example;

/// <summary>
/// Composes messages out of differently colored parts, for when the <see cref="Message.Create"/> builder gets in the way.
/// </summary>
internal static class Chat
{
    public static Message Of(params Message[] parts)
    {
        return new Message { Extra = parts };
    }

    public static Message Paint(this string text, Color color, bool bold = false)
    {
        return new Message { Text = text, Color = color, Bold = bold };
    }

    /// <summary>
    /// Splits PascalCase names into words, such as "ColdTaiga" into "Cold Taiga".
    /// </summary>
    public static string Humanize<T>(this T value) where T : struct, Enum
    {
        var name = value.ToString();
        var builder = new System.Text.StringBuilder(name.Length + 4);

        foreach (var character in name)
        {
            if (char.IsUpper(character) && builder.Length > 0)
            {
                builder.Append(' ');
            }

            builder.Append(character);
        }

        return builder.ToString();
    }
}
