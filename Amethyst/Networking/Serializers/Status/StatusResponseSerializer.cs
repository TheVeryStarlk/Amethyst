using System.Text.Json.Nodes;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Status;

namespace Amethyst.Networking.Serializers.Status;

internal sealed class StatusResponseSerializer(string status) : ISerializer<StatusResponsePacket>
{
    public int Identifier => 0;

    public int Length => Variable.GetByteCount(status);

    public static ISerializer Create(StatusResponsePacket packet)
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

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(status);
    }
}