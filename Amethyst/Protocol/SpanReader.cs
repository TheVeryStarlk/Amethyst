using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;

namespace Amethyst.Protocol;

internal ref struct SpanReader(ReadOnlySpan<byte> span)
{
    private int position;

    private readonly ReadOnlySpan<byte> span = span;

    public bool ReadBoolean()
    {
        return Unsafe.BitCast<byte, bool>(span[position++]);
    }

    public ushort ReadUnsignedShort()
    {
        return BinaryPrimitives.ReadUInt16BigEndian(span[position..(position += sizeof(ushort))]);
    }

    public double ReadDouble()
    {
        return BinaryPrimitives.ReadDoubleBigEndian(span[position..(position += sizeof(double))]);
    }

    public float ReadFloat()
    {
        return BinaryPrimitives.ReadSingleBigEndian(span[position..(position += sizeof(float))]);
    }

    public long ReadLong()
    {
        return BinaryPrimitives.ReadInt64BigEndian(span[position..(position += sizeof(long))]);
    }

    public int ReadVariableInteger()
    {
        var numbersRead = 0;
        var result = 0;

        byte read;

        do
        {
            read = span[position++];

            var value = read & 0b01111111;
            result |= value << 7 * numbersRead;

            numbersRead++;

            if (numbersRead > 5)
            {
                throw new InvalidOperationException("Variable integer is too big.");
            }
        } while ((read & 0b10000000) != 0);

        return result;
    }

    public string ReadVariableString()
    {
        var length = ReadVariableInteger();
        var buffer = span[position..(position += length)];

        return Encoding.UTF8.GetString(buffer);
    }
}