﻿using System.Buffers.Binary;
using System.Text;
using Amethyst.Api.Components;

namespace Amethyst.Networking;

internal ref struct MemoryReader(Memory<byte> memory)
{
    private readonly Span<byte> span = memory.Span;

    private int position;

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

    public ushort ReadUnsignedShort()
    {
        return BinaryPrimitives.ReadUInt16BigEndian(span[position..(position += sizeof(ushort))]);
    }

    public long ReadLong()
    {
        return BinaryPrimitives.ReadInt64BigEndian(span[position..(position += sizeof(long))]);
    }

    public bool ReadBoolean()
    {
        return span[position++] == 1;
    }

    public float ReadFloat()
    {
        return BinaryPrimitives.ReadSingleBigEndian(span[position..(position += sizeof(float))]);
    }

    public Position ReadPosition()
    {
        var value = ReadLong();
        return new Position((value >> 38), (value >> 26) & 0xFFF, (value << 38) >> 38);
    }

    private ulong ReadUnsignedLong()
    {
        return BinaryPrimitives.ReadUInt64BigEndian(span[position..(position += sizeof(ulong))]);
    }

    public byte ReadByte()
    {
        return span[position++];
    }

    public short ReadShort()
    {
        return BinaryPrimitives.ReadInt16BigEndian(span[position..(position += sizeof(short))]);
    }

    public double ReadDouble()
    {
        return BinaryPrimitives.ReadDoubleBigEndian(span[position..(position += sizeof(double))]);
    }
}