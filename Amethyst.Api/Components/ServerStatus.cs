using System.Text.Json.Serialization;

namespace Amethyst.Api.Components;

public sealed class ServerStatus
{
    public required ServerVersion Version { get; set; }

    [JsonPropertyName("players")]
    public required PlayerInformation PlayerInformation { get; set; }

    public required ChatMessage Description { get; set; }

    public string Favicon { get; set; } = string.Empty;

    public static ServerStatus Create(
        string name,
        int protocol,
        int online,
        int max,
        ChatMessage description)
    {
        return new ServerStatus
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
            Favicon = string.Empty
        };
    }
}

public sealed class ServerVersion
{
    public required string Name { get; set; }

    public required int Protocol { get; set; }
}

public sealed class PlayerInformation
{
    public required int Max { get; set; }

    public required int Online { get; set; }
}