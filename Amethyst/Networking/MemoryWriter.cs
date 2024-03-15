using System.Buffers.Binary;
using System.Globalization;
using System.Numerics;
using System.Text;

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
}