namespace Amethyst.Abstractions.Messages;

public sealed class MessageBuilder
{
    // Arbitrary length.
    private readonly Message[] extra = new Message[byte.MaxValue];

    private int index;

    internal MessageBuilder()
    {
        // Not to be used publicly.
    }

    public MessageBuilder Write(string text)
    {
        extra[index++] = Message.Create(text);
        return this;
    }

    public MessageBuilder WriteLine(string text)
    {
        extra[index++] = Message.Create($"{text}\n");
        return this;
    }

    public MessageBuilder Bold()
    {
        extra[index - 1].Bold = true;
        return this;
    }

    public MessageBuilder Italic()
    {
        extra[index - 1].Italic = true;
        return this;
    }

    public MessageBuilder Underlined()
    {
        extra[index - 1].Underlined = true;
        return this;
    }

    public MessageBuilder StrikeThrough()
    {
        extra[index - 1].StrikeThrough = true;
        return this;
    }

    public MessageBuilder Obfuscated()
    {
        extra[index - 1].Obfuscated = true;
        return this;
    }

    public MessageBuilder Black()
    {
        extra[index - 1].Color = Color.Black;
        return this;
    }

    public MessageBuilder DarkBlue()
    {
        extra[index - 1].Color = Color.DarkBlue;
        return this;
    }

    public MessageBuilder DarkGreen()
    {
        extra[index - 1].Color = Color.DarkGreen;
        return this;
    }

    public MessageBuilder DarkAqua()
    {
        extra[index - 1].Color = Color.DarkAqua;
        return this;
    }

    public MessageBuilder DarkRed()
    {
        extra[index - 1].Color = Color.DarkRed;
        return this;
    }

    public MessageBuilder DarkPurple()
    {
        extra[index - 1].Color = Color.DarkPurple;
        return this;
    }

    public MessageBuilder Gold()
    {
        extra[index - 1].Color = Color.Gold;
        return this;
    }

    public MessageBuilder Gray()
    {
        extra[index - 1].Color = Color.Gray;
        return this;
    }

    public MessageBuilder DarkGray()
    {
        extra[index - 1].Color = Color.DarkGray;
        return this;
    }

    public MessageBuilder Blue()
    {
        extra[index - 1].Color = Color.Blue;
        return this;
    }

    public MessageBuilder Green()
    {
        extra[index - 1].Color = Color.Green;
        return this;
    }

    public MessageBuilder Aqua()
    {
        extra[index - 1].Color = Color.Aqua;
        return this;
    }

    public MessageBuilder Red()
    {
        extra[index - 1].Color = Color.Red;
        return this;
    }

    public MessageBuilder LightPurple()
    {
        extra[index - 1].Color = Color.LightPurple;
        return this;
    }

    public MessageBuilder Yellow()
    {
        extra[index - 1].Color = Color.Yellow;
        return this;
    }

    public MessageBuilder White()
    {
        extra[index - 1].Color = Color.White;
        return this;
    }

    public Message Build()
    {
        return new Message { Extra = extra[..index] };
    }
}