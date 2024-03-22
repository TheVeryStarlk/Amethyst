using System.Buffers.Binary;
using System.Globalization;
using System.Numerics;
using System.Text;
using Amethyst.Api.Components;

namespace Amethyst.Networking;

internal ref struct MemoryWriter(Memory<byte> memory)
{
    public int Position { get; private set; }

    private readonly Span<byte> span = memory.Span;

    public void WriteVariableInteger(int value)
    {
        var unsigned = (uint) value;

        do
        {
            var current = (byte) (unsigned & 127);
            unsigned >>= 7;

            if (unsigned != 0)
            {
                current |= 128;
            }

            span[Position++] = current;
        } while (unsigned != 0);
    }

    public void WriteVariableString(string value)
    {
        WriteVariableInteger(Encoding.UTF8.GetByteCount(value));
        Position += Encoding.UTF8.GetBytes(value, span[Position..]);
    }

    public void WriteLong(long value)
    {
        BinaryPrimitives.WriteInt64BigEndian(span[Position..(Position += sizeof(long))], value);
    }

    public void WriteInteger(int value)
    {
        BinaryPrimitives.WriteInt32BigEndian(span[Position..(Position += sizeof(int))], value);
    }

    public void WriteByte(byte value)
    {
        span[Position++] = value;
    }

    public void WriteBoolean(bool value)
    {
        WriteByte((byte) (value ? 1 : 0));
    }

    public void WriteDouble(double value)
    {
        BinaryPrimitives.WriteDoubleBigEndian(span[Position..(Position += sizeof(double))], value);
    }

    public void WriteFloat(float value)
    {
        BinaryPrimitives.WriteSingleBigEndian(span[Position..(Position += sizeof(float))], value);
    }

    public void WriteGuid(Guid value)
    {
        var guid = BigInteger.Parse(
            value.ToString().Replace("-", ""),
            NumberStyles.HexNumber);

        Write(guid.ToByteArray(false, true));
    }

    public void Write(ReadOnlySpan<byte> value)
    {
        value.CopyTo(span[Position..(Position += value.Length)]);
    }

    public void WriteUnsignedShort(ushort value)
    {
        BinaryPrimitives.WriteUInt16BigEndian(span[Position..(Position += sizeof(ushort))], value);
    }

    public void WritePosition(Position position)
    {
        var value = ((position.X & 0x3FFFFFF) << 38) | ((position.Y & 0xFFF) << 26) | (position.Z & 0x3FFFFFF);
        WriteUnsignedLong((ulong) value);
    }

    private void WriteUnsignedLong(ulong value)
    {
        BinaryPrimitives.WriteUInt64BigEndian(span[Position..(Position += sizeof(ulong))], value);
    }

    public void WriteFixedPointInteger(int value)
    {
        BinaryPrimitives.WriteInt32BigEndian(span[Position..(Position += sizeof(int))], (int) (value * 32.0D));
    }

    public void WriteShort(short value)
    {
        BinaryPrimitives.WriteInt16BigEndian(span[Position..(Position += sizeof(short))], value);
    }
}