using System.Buffers.Binary;
using System.Text;
using Amethyst.Abstractions.Entities;

namespace Amethyst.Networking.Packets;

internal ref struct SpanReader(ReadOnlySpan<byte> span)
{
    private readonly ReadOnlySpan<byte> span = span;

    private int position;

    public byte ReadByte()
    {
        return span[position++];
    }

    public bool ReadBoolean()
    {
        return span[position++] > 0;
    }

    public short ReadShort()
    {
        return BinaryPrimitives.ReadInt16BigEndian(span[position..(position += sizeof(short))]);
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
        return Encoding.UTF8.GetString(span[position..(position += length)]);
    }

    public Position ReadPosition()
    {
        var value = ReadLong();
        return value is -1 ? new Position(-1, -1, -1) : new Position(value >> 38, value >> 26 & 0xFFF, value << 38 >> 38);
    }

    public Item ReadItem()
    {
        var type = ReadShort();

        if (type is -1)
        {
            return new Item(-1, 0, 0);
        }

        var amount = ReadByte();
        var durability = ReadShort();

        if (ReadBoolean())
        {
            throw new InvalidOperationException("Has NBT.");
        }

        return new Item(type, amount, durability);
    }
}