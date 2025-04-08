namespace Amethyst.Abstractions.Messages.Builder;

// An unnecessarily overengineered message builder.
public sealed class MessageBuilder
{
    private readonly List<Message> extra = [];

    internal MessageBuilder()
    {
        // Not to be used publicly.
    }

    public Coloring Write(string text)
    {
        extra.Add(Message.Simple(text));
        return new Coloring(this, extra);
    }

    public Coloring WriteLine(string text)
    {
        extra.Add(Message.Simple(text + "\n"));
        return new Coloring(this, extra);
    }

    public Delimiter Reset()
    {
        extra[^1] = Message.Simple("§r");
        return new Delimiter(this, extra);
    }

    public Message Build()
    {
        return new Message { Extra = extra };
    }
}