using System.Text.Json.Nodes;
using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Networking.Packets.Status;

public sealed class StatusResponsePacket(string name, int numerical, int maximum, int online, Message description, string favicon) : IOutgoingPacket
{
    public int Identifier => 0;

    public int Length
    {
        get
        {
            var version = new JsonObject
            {
                ["name"] = name,
                ["protocol"] = numerical
            };

            var players = new JsonObject
            {
                ["max"] = maximum,
                ["online"] = online
            };

            var parent = new JsonObject
            {
                ["version"] = version,
                ["players"] = players,
                ["description"] = JsonNode.Parse(description.Serialize()),
                ["favicon"] = favicon
            };

            serialized = parent.ToJsonString();

            return Variable.GetByteCount(serialized);
        }
    }

    private string? serialized;

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(serialized!);
    }
}