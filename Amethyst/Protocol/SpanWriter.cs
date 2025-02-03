using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;

namespace Amethyst.Protocol;

internal ref struct SpanWriter(Span<byte> span)
{
    private int position;

    private readonly Span<byte> span = span;

    public void Write(ReadOnlySpan<byte> value)
    {
        value.CopyTo(span[position..(position += value.Length)]);
    }

    public void WriteByte(byte value)
    {
        span[position++] = value;
    }

    public void WriteBoolean(bool value)
    {
        WriteByte(Unsafe.BitCast<bool, byte>(value));
    }

    public void WriteInteger(int value)
    {
        BinaryPrimitives.WriteInt32BigEndian(span[position..(position += sizeof(int))], value);
    }

    public void WriteLong(long value)
    {
        BinaryPrimitives.WriteInt64BigEndian(span[position..(position += sizeof(long))], value);
    }

    public void WriteDouble(double value)
    {
        BinaryPrimitives.WriteDoubleBigEndian(span[position..(position += sizeof(double))], value);
    }

    public void WriteFloat(float value)
    {
        BinaryPrimitives.WriteSingleBigEndian(span[position..(position += sizeof(float))], value);
    }

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

            span[position++] = current;
        } while (unsigned != 0);
    }

    public void WriteVariableString(string value)
    {
        WriteVariableInteger(Encoding.UTF8.GetByteCount(value));
        position += Encoding.UTF8.GetBytes(value, span[position..]);
    }
}