using System.Text.Json.Serialization;
using Amethyst.Components.Messages;

namespace Amethyst.Components.Status;

public sealed record Status(
    Version Version,
    [property: JsonPropertyName("players")]
    Information Information,
    Message Description,
    string Favicon);