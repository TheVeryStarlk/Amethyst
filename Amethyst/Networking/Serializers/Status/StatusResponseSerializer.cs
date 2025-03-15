using System.Text.Json.Nodes;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Status;

namespace Amethyst.Networking.Serializers.Status;

internal sealed class StatusResponseSerializer(StatusResponsePacket packet) : Serializer(packet)
{
    public override int Identifier => 0;

    public override int Length
    {
        get
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

            serialized = parent.ToJsonString();

            return Variable.GetByteCount(serialized);
        }
    }

    private string? serialized;

    public override void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(serialized!);
    }
}