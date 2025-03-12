namespace Amethyst.Abstractions.Messages.Builder;

// An unnecessarily overengineered message builder.
public sealed class MessageBuilder
{
    // Arbitrary length.
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

    public MessageBuilder Reset()
    {
        extra[^1] = Message.Simple("§r");
        return this;
    }

    public Message Build()
    {
        return new Message { Extra = extra };
    }
}