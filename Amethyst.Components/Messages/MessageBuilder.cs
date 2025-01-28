namespace Amethyst.Components.Messages;

public sealed class MessageBuilder
{
    private readonly List<Message> extras = [];

    internal MessageBuilder()
    {
        // Not to be used publicly.
    }

    public MessageBuilder Write(
        string text,
        bool bold = false,
        bool italic = false,
        bool underlined = false,
        bool strikeThrough = false,
        bool obfuscated = false,
        Color color = Color.White)
    {
        extras.Add(Message.Create(text, bold, italic, underlined, strikeThrough, obfuscated, color));
        return this;
    }

    public Message Build()
    {
        return new Message
        {
            Extra = extras
        };
    }
}