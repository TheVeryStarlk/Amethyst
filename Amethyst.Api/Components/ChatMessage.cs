﻿namespace Amethyst.Api.Components;

public sealed class ChatMessage
{
    public string Text { get; set; } = string.Empty;

    public bool Bold { get; set; }

    public bool Italic { get; set; }

    public bool Underlined { get; set; }

    public bool StrikeThrough { get; set; }

    public bool Obfuscated { get; set; }

    public string Color { get; set; } = "white";

    public ChatMessage[]? Extra { get; set; }

    public static ChatMessage Create(
        string text,
        bool bold = false,
        bool italic = false,
        bool underlined = false,
        bool strikeThrough = false,
        bool obfuscated = false,
        string color = "white")
    {
        return new ChatMessage
        {
            Text = text,
            Bold = bold,
            Italic = italic,
            Underlined = underlined,
            StrikeThrough = strikeThrough,
            Obfuscated = obfuscated,
            Color = color
        };
    }
}