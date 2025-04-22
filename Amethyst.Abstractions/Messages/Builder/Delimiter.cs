namespace Amethyst.Abstractions.Messages.Builder;

public sealed class Delimiter
{
    private readonly MessageBuilder parent;
    private readonly List<Message> extra;

    internal Delimiter(MessageBuilder parent, List<Message> extra)
    {
        this.parent = parent;
        this.extra = extra;
    }

    public Coloring Write(string text)
    {
        extra.Add(Message.Simple(text));
        return new Coloring(parent, extra);
    }

    public Coloring WriteLine(string text)
    {
        extra.Add(Message.Simple(text + "\n"));
        return new Coloring(parent, extra);
    }

    public Message Build()
    {
        return parent.Build();
    }
}