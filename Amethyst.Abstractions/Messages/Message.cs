using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Amethyst.Abstractions.Messages.Builder;

namespace Amethyst.Abstractions.Messages;

public sealed class Message
{
    public string Text { get; set; } = string.Empty;

    public bool Bold { get; set; }

    public bool Italic { get; set; }

    public bool Underlined { get; set; }

    public bool StrikeThrough { get; set; }

    public bool Obfuscated { get; set; }

    public Color Color { get; set; } = Color.White;

    public IEnumerable<Message>? Extra { get; set; }

    public static implicit operator Message(string text)
    {
        return new Message { Text = text };
    }

    public static Message Simple(string text)
    {
        return new Message { Text = text };
    }

    public static MessageBuilder Create()
    {
        return new MessageBuilder();
    }

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "https://github.com/dotnet/runtime/issues/51544#issuecomment-1516232559")]
    [UnconditionalSuppressMessage("AOT", "IL3050", Justification = "https://github.com/dotnet/runtime/issues/51544#issuecomment-1516232559")]
    public string Serialize()
    {
        return JsonSerializer.Serialize(this, JsonSerializerExtensions.Options);
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