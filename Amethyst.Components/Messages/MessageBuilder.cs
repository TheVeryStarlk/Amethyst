namespace Amethyst.Components.Messages;

public sealed class MessageBuilder
{
    private readonly List<Message> extra = [];

    internal MessageBuilder()
    {
        // Not to be used publicly.
    }

    public MessageBuilder Write(string text)
    {
        extra.Add(Message.Create(text));
        return this;
    }

    public MessageBuilder WriteLine(string text)
    {
        extra.Add(Message.Create($"{text}\n"));
        return this;
    }

    public MessageBuilder Bold()
    {
        extra[^1].Bold = true;
        return this;
    }

    public MessageBuilder Italic()
    {
        extra[^1].Italic = true;
        return this;
    }

    public MessageBuilder Underlined()
    {
        extra[^1].Underlined = true;
        return this;
    }

    public MessageBuilder StrikeThrough()
    {
        extra[^1].StrikeThrough = true;
        return this;
    }

    public MessageBuilder Obfuscated()
    {
        extra[^1].Obfuscated = true;
        return this;
    }

    public MessageBuilder Black()
    {
        extra[^1].Color = Color.Black;
        return this;
    }

    public MessageBuilder DarkBlue()
    {
        extra[^1].Color = Color.DarkBlue;
        return this;
    }

    public MessageBuilder DarkGreen()
    {
        extra[^1].Color = Color.DarkGreen;
        return this;
    }

    public MessageBuilder DarkAqua()
    {
        extra[^1].Color = Color.DarkAqua;
        return this;
    }

    public MessageBuilder DarkRed()
    {
        extra[^1].Color = Color.DarkRed;
        return this;
    }

    public MessageBuilder DarkPurple()
    {
        extra[^1].Color = Color.DarkPurple;
        return this;
    }

    public MessageBuilder Gold()
    {
        extra[^1].Color = Color.Gold;
        return this;
    }

    public MessageBuilder Gray()
    {
        extra[^1].Color = Color.Gray;
        return this;
    }

    public MessageBuilder DarkGray()
    {
        extra[^1].Color = Color.DarkGray;
        return this;
    }

    public MessageBuilder Blue()
    {
        extra[^1].Color = Color.Blue;
        return this;
    }

    public MessageBuilder Green()
    {
        extra[^1].Color = Color.Green;
        return this;
    }

    public MessageBuilder Aqua()
    {
        extra[^1].Color = Color.Aqua;
        return this;
    }

    public MessageBuilder Red()
    {
        extra[^1].Color = Color.Red;
        return this;
    }

    public MessageBuilder LightPurple()
    {
        extra[^1].Color = Color.LightPurple;
        return this;
    }

    public MessageBuilder Yellow()
    {
        extra[^1].Color = Color.Yellow;
        return this;
    }

    public MessageBuilder White()
    {
        extra[^1].Color = Color.White;
        return this;
    }

    public Message Build()
    {
        return new Message { Extra = extra };
    }
}