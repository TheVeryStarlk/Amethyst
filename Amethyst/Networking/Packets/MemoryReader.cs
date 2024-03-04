using System.Buffers.Binary;
using System.Text;

namespace Amethyst.Networking.Packets;

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

    public string ReadString()
    {
        var length = ReadVariableInteger();
        var buffer = span[position..(position += length)];
        return Encoding.UTF8.GetString(buffer);
    }

    public ushort ReadUnsignedShort()
    {
        return BinaryPrimitives.ReadUInt16BigEndian(span[position..(position += sizeof(ushort))]);
    }
}