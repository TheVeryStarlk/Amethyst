namespace Amethyst.Api.Components;

public sealed class ChatMessage
{
    public string Text { get; set; } = string.Empty;

    public bool Bold { get; set; }

    public bool Italic { get; set; }

    public bool Underlined { get; set; }

    public bool StrikeThrough { get; set; }

    public bool Obfuscated { get; set; }

    public Color Color { get; set; } = Color.White;

    public ChatMessage[]? Extra { get; set; }

    public static ChatMessage Create(
        string text,
        Color color = Color.White)
    {
        return new ChatMessage
        {
            Text = text,
            Color = color
        };
    }

    public static implicit operator ChatMessage(string value)
    {
        var text = string.Create(value.Length, value, (span, text) =>
        {
            for (var index = 0; index < span.Length; index++)
            {
                var character = text[index];
                span[index] = character;

                if (character != '&' || index + 1 >= text.Length)
                {
                    continue;
                }

                character = text[index + 1];

                span[index] = character switch
                {
                    >= '0' and <= '9' => '§',
                    >= 'a' and <= 'e' => '§',
                    'k' or 'l' or 'm' or 'n' or 'o' or 'r' => '§',
                    _ => span[index]
                };
            }
        });

        return new ChatMessage
        {
            Text = text
        };
    }
}

public enum Color
{
    Black,
    DarkBlue,
    DarkGreen,
    DarkAqua,
    DarkRed,
    DarkPurple,
    Gold,
    Gray,
    DarkGray,
    Blue,
    Green,
    Aqua,
    Red,
    LightPurple,
    Yellow,
    White
}