namespace Amethyst.Abstractions.Messages.Builder;

public sealed class Coloring
{
    private readonly MessageBuilder parent;
    private readonly List<Message> extra;

    internal Coloring(MessageBuilder parent, List<Message> extra)
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

    public Formatting Black()
    {
        extra[^1].Color = Color.Black;
        return new Formatting(parent, extra);
    }

    public Formatting DarkBlue()
    {
        extra[^1].Color = Color.DarkBlue;
        return new Formatting(parent, extra);
    }

    public Formatting DarkGreen()
    {
        extra[^1].Color = Color.DarkGreen;
        return new Formatting(parent, extra);
    }

    public Formatting DarkAqua()
    {
        extra[^1].Color = Color.DarkAqua;
        return new Formatting(parent, extra);
    }

    public Formatting DarkRed()
    {
        extra[^1].Color = Color.DarkRed;
        return new Formatting(parent, extra);
    }

    public Formatting DarkPurple()
    {
        extra[^1].Color = Color.DarkPurple;
        return new Formatting(parent, extra);
    }

    public Formatting Gold()
    {
        extra[^1].Color = Color.Gold;
        return new Formatting(parent, extra);
    }

    public Formatting Gray()
    {
        extra[^1].Color = Color.Gray;
        return new Formatting(parent, extra);
    }

    public Formatting DarkGray()
    {
        extra[^1].Color = Color.DarkGray;
        return new Formatting(parent, extra);
    }

    public Formatting Blue()
    {
        extra[^1].Color = Color.Blue;
        return new Formatting(parent, extra);
    }

    public Formatting Green()
    {
        extra[^1].Color = Color.Green;
        return new Formatting(parent, extra);
    }

    public Formatting Aqua()
    {
        extra[^1].Color = Color.Aqua;
        return new Formatting(parent, extra);
    }

    public Formatting Red()
    {
        extra[^1].Color = Color.Red;
        return new Formatting(parent, extra);
    }

    public Formatting LightPurple()
    {
        extra[^1].Color = Color.LightPurple;
        return new Formatting(parent, extra);
    }

    public Formatting Yellow()
    {
        extra[^1].Color = Color.Yellow;
        return new Formatting(parent, extra);
    }

    public Formatting White()
    {
        extra[^1].Color = Color.White;
        return new Formatting(parent, extra);
    }
}