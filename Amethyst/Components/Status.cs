using System.Text.Json.Serialization;

namespace Amethyst.Components;

internal sealed class Status
{
    public required ServerVersion Version { get; set; }

    [JsonPropertyName("players")]
    public required PlayerInformation PlayerInformation { get; set; }

    public required Chat Description { get; set; }

    public string Favicon { get; set; } = string.Empty;

    public static Status Create(
        string name,
        int protocol,
        int max,
        int online,
        Chat description,
        string favicon = "")
    {
        return new Status
        {
            Version = new ServerVersion
            {
                Name = name,
                Protocol = protocol
            },

            PlayerInformation = new PlayerInformation
            {
                Max = max,
                Online = online
            },

            Description = description,
            Favicon = favicon
        };
    }
}

internal sealed class ServerVersion
{
    public required string Name { get; set; }

    public required int Protocol { get; set; }
}

internal sealed class PlayerInformation
{
    public required int Max { get; set; }

    public required int Online { get; set; }
}