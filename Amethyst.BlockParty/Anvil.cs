using System.Buffers.Binary;
using System.IO.Compression;
using Amethyst.Abstractions.Worlds;
using Amethyst.BlockParty.Serializer;
using Amethyst.BlockParty.Serializer.Tags;

namespace Amethyst.BlockParty;

internal static class Anvil
{
    public static int Load(string directory, IWorld world)
    {
        var options = new BinaryTagSerializerOptions { MaximumDepth = Array.MaxLength };
        var paths = Directory.GetFiles(directory);

        using var output = new MemoryStream();

        foreach (var path in paths)
        {
            var region = File.ReadAllBytes(path);

            for (var index = 0; index < 4096;)
            {
                var slice = region[index..(index += 4)];
                var entry = BinaryPrimitives.ReadInt32BigEndian(slice);

                var offset = (entry >> 8 & 0xFFFFFF) * 4096;
                var size = (entry & 0xFF) * 4096;

                if (size is 0)
                {
                    continue;
                }

                var current = region[offset..];

                using var stream = new MemoryStream(current[(sizeof(int) + sizeof(byte))..]);

                output.Position = 0;

                using var decompressor = new ZLibStream(stream, CompressionMode.Decompress);

                decompressor.CopyTo(output);

                var tag = BinaryTagSerializer.Deserialize<CompoundTag>(output.ToArray(), options);

                var chunk = Chunk.Create(tag.First<CompoundTag>());

                Parallel.For(
                    0,
                    16 * 256 * 16,
                    integer =>
                    {
                        var x = integer % 16;
                        var y = integer / 16 % 256;
                        var z = integer / (16 * 256);

                        world[chunk.X, chunk.Z][x, y, z] = chunk[x, y, z];
                    });

                // for (var x = 0; x < 16; x++)
                // {
                //     for (var y = 0; y < 256; y++)
                //     {
                //         for (var z = 0; z < 16; z++)
                //         {
                //             world[chunk.X, chunk.Z][x, y, z] = chunk[x, y, z];
                //         }
                //     }
                // }
            }
        }

        return paths.Length;
    }
}

internal sealed class Chunk(int horizontal, int vertical, Section[] sections)
{
    public int X => horizontal;

    public int Z => vertical;

    public Block this[int x, int y, int z] => sections.Length > y.ToChunk() ? sections[y.ToChunk()][x, y, z] : Blocks.Air;

    public static Chunk Create(CompoundTag tag)
    {
        var x = tag.First<IntegerTag>("xPos").Value;
        var z = tag.First<IntegerTag>("zPos").Value;

        var sections = Section.Read(tag.First<ListTag>("Sections"));

        return new Chunk(x, z, sections);
    }
}

internal readonly struct Section(byte[] blocks, ReadOnlyNibbleArray array)
{
    public Block this[int x, int y, int z]
    {
        get
        {
            var index = NumericUtility.AsIndex(x, y, z);
            return new Block(blocks[index], array[index]);
        }
    }

    public static Section[] Read(ListTag tag)
    {
        var sections = new Section[tag.Children.Length];

        for (var index = 0; index < sections.Length; index++)
        {
            var current = (CompoundTag) tag.Children[index];

            sections[index] = new Section(
                current.First<ByteCollectionTag>("Blocks").Children,
                new ReadOnlyNibbleArray(current.First<ByteCollectionTag>("Data").Children));
        }

        return sections;
    }
}

internal readonly struct ReadOnlyNibbleArray(byte[] array)
{
    public byte this[int index] => (byte) (array[index / 2] >> index % 2 * 4 & 0xF);
}

internal static class NumericUtility
{
    public static int ToChunk(this int value)
    {
        return value >> 4;
    }

    public static int AsIndex(int x, int y, int z)
    {
        return (y & 0xF) << 8 | (z & 0xF) << 4 | x & 0xF;
    }
}