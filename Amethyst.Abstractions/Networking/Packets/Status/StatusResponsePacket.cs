using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Networking.Packets.Status;

public sealed class StatusResponsePacket(string name, int numerical, int maximum, int online, Message description, string favicon) : IOutgoingPacket
{
    public int Length
    {
        get
        {
            var status = new
            {
                Version = new
                {
                    Name = name,
                    Protocol = numerical
                },
                Players = new
                {
                    Max = maximum,
                    Online = online
                },
                Description = description.Serialize(),
                Favicon = favicon
            };

            serialized = status.Serialize();

            return Variable.GetByteCount(serialized);
        }
    }

    private string? serialized;

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(serialized!);
    }
}