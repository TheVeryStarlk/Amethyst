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
        return Create(value.Replace('&', '§'));
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