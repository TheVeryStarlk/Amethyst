using System.Text.Json.Serialization;

namespace Amethyst.Abstractions.Messages;

public sealed class Status
{
    public Version Version { get; }

    [property: JsonPropertyName("players")]
    public Information Information { get; }

    public Message Description { get; }

    public string Favicon { get; }

    private Status(Version version, Information information, Message description, string favicon)
    {
        Version = version;
        Information = information;
        Description = description;
        Favicon = favicon;
    }

    public static Status Create(string name, int numerical, int maximum, int online, Message description, string favicon)
    {
        return new Status(new Version(name, numerical), new Information(maximum, online), description, favicon);
    }
}

public sealed class Version
{
    public string Name { get; }

    [property: JsonPropertyName("protocol")]
    public int Numerical { get; }

    internal Version(string name, int numerical)
    {
        Name = name;
        Numerical = numerical;
    }
}

public sealed class Information
{
    public int Maximum { get; }

    [property: JsonPropertyName("max")]
    public int Online { get; }

    internal Information(int maximum, int online)
    {
        Maximum = maximum;
        Online = online;
    }
}