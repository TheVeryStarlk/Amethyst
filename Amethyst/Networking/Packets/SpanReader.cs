using System.Buffers.Binary;
using System.Text;
using Amethyst.Abstractions.Entities;

namespace Amethyst.Networking.Packets;

internal ref struct SpanReader(ReadOnlySpan<byte> span)
{
    private readonly ReadOnlySpan<byte> span = span;

    public int Position { get; private set; }

    public ReadOnlySpan<byte> Read(int length)
    {
        return span[Position..(Position += length)];
    }

    public byte ReadByte()
    {
        return span[Position++];
    }

    public bool ReadBoolean()
    {
        return span[Position++] > 0;
    }

    public short ReadShort()
    {
        return BinaryPrimitives.ReadInt16BigEndian(span[Position..(Position += sizeof(short))]);
    }

    public ushort ReadUnsignedShort()
    {
        return BinaryPrimitives.ReadUInt16BigEndian(span[Position..(Position += sizeof(ushort))]);
    }

    public double ReadDouble()
    {
        return BinaryPrimitives.ReadDoubleBigEndian(span[Position..(Position += sizeof(double))]);
    }

    public float ReadFloat()
    {
        return BinaryPrimitives.ReadSingleBigEndian(span[Position..(Position += sizeof(float))]);
    }

    public long ReadLong()
    {
        return BinaryPrimitives.ReadInt64BigEndian(span[Position..(Position += sizeof(long))]);
    }

    public int ReadVariableInteger()
    {
        var numbersRead = 0;
        var result = 0;

        byte read;

        do
        {
            read = span[Position++];

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
        return Encoding.UTF8.GetString(span[Position..(Position += length)]);
    }

    public Position ReadPosition()
    {
        var value = ReadLong();
        return value is -1 ? new Position(-1, -1, -1) : new Position(value >> 38, value >> 26 & 0xFFF, value << 38 >> 38);
    }
}