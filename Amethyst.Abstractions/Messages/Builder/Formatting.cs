namespace Amethyst.Abstractions.Messages.Builder;

public sealed class Formatting
{
    private readonly MessageBuilder parent;
    private readonly List<Message> extra;

    internal Formatting(MessageBuilder parent, List<Message> extra)
    {
        this.parent = parent;
        this.extra = extra;
    }

    public MessageBuilder Write(string text)
    {
        extra.Add(Message.Simple(text));
        return parent;
    }

    public MessageBuilder WriteLine(string text)
    {
        extra.Add(Message.Simple(text + "\n"));
        return parent;
    }

    public MessageBuilder Bold()
    {
        extra[^1].Bold = true;
        return parent;
    }

    public MessageBuilder Italic()
    {
        extra[^1].Italic = true;
        return parent;
    }

    public MessageBuilder Underlined()
    {
        extra[^1].Underlined = true;
        return parent;
    }

    public MessageBuilder StrikeThrough()
    {
        extra[^1].StrikeThrough = true;
        return parent;
    }

    public MessageBuilder Obfuscated()
    {
        extra[^1].Obfuscated = true;
        return parent;
    }
}