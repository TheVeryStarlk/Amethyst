namespace Amethyst.Components.Messages;

public sealed class Message
{
    public string Text { get; set; } = string.Empty;

    public bool Bold { get; set; }

    public bool Italic { get; set; }

    public bool Underlined { get; set; }

    public bool StrikeThrough { get; set; }

    public bool Obfuscated { get; set; }

    public Color Color { get; set; } = Color.White;

    public IEnumerable<Message>? Extra { get; set; }

    public static implicit operator Message(string text)
    {
        var extra = new List<Message>();
        var index = 0;

        while (index < text.Length)
        {
            if (text[index] is not '&')
            {
                index++;
                continue;
            }

            var start = index++;

            while (index < text.Length)
            {
                if (text[index] is '&')
                {
                    break;
                }

                index++;
            }

            var part = text[start..index];

            if (part.Length < 2)
            {
                extra.Add(new Message { Text = part });
                continue;
            }

            var prefix = char.ToLower(part[1]);
            var previous = extra.Count > 0 ? extra.Last() : new Message();

            var kind = prefix switch
            {
                >= '0' and <= '9' => (Color) int.Parse(prefix.ToString()),
                'a' => Color.Green,
                'b' => Color.Aqua,
                'c' => Color.Red,
                'd' => Color.LightPurple,
                'e' => Color.Yellow,
                'f' => Color.White,
                _ => previous.Color
            };

            var current = new Message
            {
                Text = text[(start + 2)..index],
                Obfuscated = prefix is 'k' || previous.Obfuscated,
                Bold = prefix is 'l'|| previous.Bold,
                StrikeThrough = prefix is 'm'|| previous.StrikeThrough,
                Underlined = prefix is 'n'|| previous.Underlined,
                Italic = prefix is 'o'|| previous.Italic,
                Color = kind
            };

            extra.Add(current);
        }

        return new Message { Extra = extra };
    }

    public static Message Create(
        string text,
        bool bold = false,
        bool italic = false,
        bool underlined = false,
        bool strikeThrough = false,
        bool obfuscated = false,
        Color color = Color.White)
    {
        return new Message
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

    public static MessageBuilder Create()
    {
        return new MessageBuilder();
    }
}

public enum Color
{
    Black,
    DarkBlue,
    DarkGreen,
    DarkAqua,
    DarkRed,
    DarkPurple,
    Gold,
    Gray,
    DarkGray,
    Blue,
    Green,
    Aqua,
    Red,
    LightPurple,
    Yellow,
    White
}