using System.Text.Json.Nodes;
using Amethyst.Abstractions.Packets.Status;

namespace Amethyst.Networking.Serializers.Status;

internal sealed partial class StatusResponseSerializer(string status) : ISerializer<StatusResponsePacket, StatusResponseSerializer>
{
    public static StatusResponseSerializer Create(StatusResponsePacket packet)
    {
        var version = new JsonObject
        {
            ["name"] = packet.Name,
            ["protocol"] = packet.Numerical
        };

        var players = new JsonObject
        {
            ["max"] = packet.Maximum,
            ["online"] = packet.Online
        };

        var parent = new JsonObject
        {
            ["version"] = version,
            ["players"] = players,
            ["description"] = JsonNode.Parse(packet.Description.Serialize()),
            ["favicon"] = packet.Favicon
        };

        return new StatusResponseSerializer(parent.ToJsonString());
    }
}