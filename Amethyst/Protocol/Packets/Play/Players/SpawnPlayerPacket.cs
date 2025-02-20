using Amethyst.Components.Entities;
using Amethyst.Components.Protocol;

namespace Amethyst.Protocol.Packets.Play.Players;

public sealed record SpawnPlayerPacket(IPlayer Player) : IOutgoingPacket
{
    public int Identifier => 12;

    public int Length => Variable.GetByteCount(Player.Client.Identifier)
                         + sizeof(long)
                         + sizeof(long)
                         + sizeof(int)
                         + sizeof(int)
                         + sizeof(int)
                         + sizeof(byte)
                         + sizeof(byte)
                         + sizeof(short)
                         + sizeof(byte);

    public void Write(Span<byte> span)
    {
        var absolute = Player.Location.ToAbsolute();

        SpanWriter
            .Create(span)
            .WriteVariableInteger(Player.Identifier)
            .Write(Player.Guid.ToArray())
            .WriteInteger((int) absolute.X)
            .WriteInteger((int) absolute.Y)
            .WriteInteger((int) absolute.Z)
            .WriteByte(Player.Yaw.ToAbsolute())
            .WriteByte(Player.Pitch.ToAbsolute())
            .WriteShort(0)
            .WriteByte(127);
    }
}