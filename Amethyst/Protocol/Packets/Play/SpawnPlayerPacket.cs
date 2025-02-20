using System.Globalization;
using System.Numerics;
using Amethyst.Components.Entities;
using Amethyst.Components.Protocol;

namespace Amethyst.Protocol.Packets.Play;

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
        SpanWriter
            .Create(span)
            .WriteVariableInteger(Player.Identifier)
            .Write(BigInteger.Parse(Player.Guid.ToString().Replace("-", ""), NumberStyles.HexNumber).ToByteArray(isBigEndian: true))
            .WriteInteger((int) (Player.Location.X * 32D))
            .WriteInteger((int) (Player.Location.Y * 32D))
            .WriteInteger((int) (Player.Location.Z * 32D))
            .WriteByte((byte) (Player.Yaw / 256))
            .WriteByte((byte) (Player.Pitch / 256))
            .WriteShort(0)
            .WriteByte(127);
    }
}