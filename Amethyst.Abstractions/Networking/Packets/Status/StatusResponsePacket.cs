using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Networking.Packets.Status;

public sealed class StatusResponsePacket(string name, int numerical, int maximum, int online, Message description, string favicon) : IOutgoingPacket
{
    public int Length { get; }

    public void Write(Span<byte> span)
    {
        throw new NotImplementedException();
    }
}