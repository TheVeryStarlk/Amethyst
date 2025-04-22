using System.Buffers.Binary;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using Amethyst.Abstractions.Entities;

namespace Amethyst.Networking;

internal ref struct SpanWriter
{
    private int position;

    private readonly Span<byte> span;

    private SpanWriter(Span<byte> span)
    {
        this.span = span;
    }

    public static SpanWriter Create(Span<byte> span)
    {
        return new SpanWriter(span);
    }

    public SpanWriter Write(ReadOnlySpan<byte> value)
    {
        value.CopyTo(span[position..(position += value.Length)]);
        return this;
    }

    public SpanWriter WriteByte(byte value)
    {
        span[position++] = value;
        return this;
    }

    public SpanWriter WriteFixedByte(double value)
    {
        WriteByte((byte) (value * 32D));
        return this;
    }

    public SpanWriter WriteAngle(float value)
    {
        WriteByte((byte) (value % 360 / 360 * 256));
        return this;
    }

    public SpanWriter WriteBoolean(bool value)
    {
        WriteByte(Unsafe.BitCast<bool, byte>(value));
        return this;
    }

    public SpanWriter WriteShort(short value)
    {
        BinaryPrimitives.WriteInt16BigEndian(span[position..(position += sizeof(short))], value);
        return this;
    }

    public SpanWriter WriteUnsignedShort(ushort value)
    {
        BinaryPrimitives.WriteUInt16BigEndian(span[position..(position += sizeof(ushort))], value);
        return this;
    }

    public SpanWriter WriteInteger(int value)
    {
        BinaryPrimitives.WriteInt32BigEndian(span[position..(position += sizeof(int))], value);
        return this;
    }

    public SpanWriter WriteLong(long value)
    {
        BinaryPrimitives.WriteInt64BigEndian(span[position..(position += sizeof(long))], value);
        return this;
    }

    public SpanWriter WriteDouble(double value)
    {
        BinaryPrimitives.WriteDoubleBigEndian(span[position..(position += sizeof(double))], value);
        return this;
    }

    public SpanWriter WriteFixedDouble(double value)
    {
        WriteInteger((int) (value * 32D));
        return this;
    }

    public SpanWriter WriteFloat(float value)
    {
        BinaryPrimitives.WriteSingleBigEndian(span[position..(position += sizeof(float))], value);
        return this;
    }

    public SpanWriter WriteVariableInteger(int value)
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

        return this;
    }

    public SpanWriter WriteVariableString(string value)
    {
        WriteVariableInteger(Encoding.UTF8.GetByteCount(value));
        position += Encoding.UTF8.GetBytes(value, span[position..]);

        return this;
    }

    public SpanWriter WritePosition(Position value)
    {
        WriteLong(((long) value.X & 0x3FFFFFF) << 38 | ((long) value.Y & 0xFFF) << 26 | (long) value.Z & 0x3FFFFFF);
        return this;
    }

    public SpanWriter WriteGuid(Guid value)
    {
        Write(BigInteger.Parse(value.ToString().Replace("-", ""), NumberStyles.HexNumber).ToByteArray(isBigEndian: true));
        return this;
    }
}