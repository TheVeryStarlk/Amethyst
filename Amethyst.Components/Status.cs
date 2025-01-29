using System.Text.Json.Serialization;
using Amethyst.Components.Messages;

namespace Amethyst.Components;

public sealed record Status(
    Version Version,
    [property: JsonPropertyName("players")]
    Information Information,
    Message Description,
    string Favicon);

public sealed class Version(string Name, int Protocol);

public sealed record Information(int Maximum, int Online);