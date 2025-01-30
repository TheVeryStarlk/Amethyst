namespace Amethyst.Components.Messages;

public sealed class MessageBuilder
{
    private readonly List<Message> extras = [];

    internal MessageBuilder()
    {
        // Not to be used publicly.
    }

    public MessageBuilder Write(string text)
    {
        extras.Add(Message.Create(text));
        return this;
    }

    public MessageBuilder WriteLine(string text)
    {
        extras.Add(Message.Create($"{text}\n"));
        return this;
    }

    public MessageBuilder Bold()
    {
        extras[^1].Bold = true;
        return this;
    }

    public MessageBuilder Italic()
    {
        extras[^1].Italic = true;
        return this;
    }

    public MessageBuilder Underlined()
    {
        extras[^1].Underlined = true;
        return this;
    }

    public MessageBuilder StrikeThrough()
    {
        extras[^1].StrikeThrough = true;
        return this;
    }

    public MessageBuilder Obfuscated()
    {
        extras[^1].Obfuscated = true;
        return this;
    }

    public MessageBuilder Black()
    {
        extras[^1].Color = Color.Black;
        return this;
    }

    public MessageBuilder DarkBlue()
    {
        extras[^1].Color = Color.DarkBlue;
        return this;
    }

    public MessageBuilder DarkGreen()
    {
        extras[^1].Color = Color.DarkGreen;
        return this;
    }

    public MessageBuilder DarkAqua()
    {
        extras[^1].Color = Color.DarkAqua;
        return this;
    }

    public MessageBuilder DarkRed()
    {
        extras[^1].Color = Color.DarkRed;
        return this;
    }

    public MessageBuilder DarkPurple()
    {
        extras[^1].Color = Color.DarkPurple;
        return this;
    }

    public MessageBuilder Gold()
    {
        extras[^1].Color = Color.Gold;
        return this;
    }

    public MessageBuilder Gray()
    {
        extras[^1].Color = Color.Gray;
        return this;
    }

    public MessageBuilder DarkGray()
    {
        extras[^1].Color = Color.DarkGray;
        return this;
    }

    public MessageBuilder Blue()
    {
        extras[^1].Color = Color.Blue;
        return this;
    }

    public MessageBuilder Green()
    {
        extras[^1].Color = Color.Green;
        return this;
    }

    public MessageBuilder Aqua()
    {
        extras[^1].Color = Color.Aqua;
        return this;
    }

    public MessageBuilder Red()
    {
        extras[^1].Color = Color.Red;
        return this;
    }

    public MessageBuilder LightPurple()
    {
        extras[^1].Color = Color.LightPurple;
        return this;
    }

    public MessageBuilder Yellow()
    {
        extras[^1].Color = Color.Yellow;
        return this;
    }

    public MessageBuilder White()
    {
        extras[^1].Color = Color.White;
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