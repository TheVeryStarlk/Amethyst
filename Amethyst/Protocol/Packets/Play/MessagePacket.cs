using Amethyst.Abstractions.Eventing.Sources.Player;

namespace Amethyst.Protocol.Packets.Play;

internal sealed record MessagePacket(string Message, byte Position) : IIngoingPacket<MessagePacket>, IOutgoingPacket
{
    public static int Identifier => 1;

    int IOutgoingPacket.Identifier => 2;

    public int Length => Variable.GetByteCount(Message) + sizeof(byte);

    public static MessagePacket Create(SpanReader reader)
    {
        return new MessagePacket(reader.ReadVariableString(), 0);
    }

    public void Write(ref SpanWriter writer)
    {
        writer.WriteVariableString(Message);
        writer.WriteByte(Position);
    }

    public async ValueTask Handle(Client client)
    {
        await client.EventDispatcher.DispatchAsync(client.Player!, new Sent(Message), client.CancellationToken).ConfigureAwait(false);
    }
}