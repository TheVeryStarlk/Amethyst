using System.Text.Json.Serialization;
using Amethyst.Components.Messages;

namespace Amethyst.Components;

public sealed record Status(
    Version Version,
    [property: JsonPropertyName("players")]
    Information Information,
    Message Description,
    string Favicon)
{
    public static Status Create(string name, int numerical, int maximum, int online, Message description, string favicon)
    {
        return new Status(new Version(name, numerical), new Information(maximum, online), description, favicon);
    }
}

public sealed record Version(
    string Name,
    [property: JsonPropertyName("protocol")]
    int Numerical);

public sealed record Information(
    [property: JsonPropertyName("max")]
    int Maximum,
    int Online);