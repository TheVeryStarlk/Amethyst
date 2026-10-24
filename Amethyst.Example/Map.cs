using System.Buffers.Binary;
using System.IO.Compression;
using System.Text;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Example;

/// <summary>
/// Renders a top-down map of a world into a PNG image, shaded like the in-game maps.
/// </summary>
internal static class Map
{
    private static readonly uint[] table = CreateTable();

    /// <param name="realm">The realm whose world to render, missing chunks are generated.</param>
    /// <param name="x">The X position at the center of the map.</param>
    /// <param name="z">The Z position at the center of the map.</param>
    /// <param name="size">The width and height of the image, in pixels.</param>
    /// <param name="scale">How many blocks each pixel covers.</param>
    public static byte[] Render(Realm realm, int x, int z, int size, int scale)
    {
        var pixels = new byte[size * size * 3];
        var previous = new int[size];

        // One extra row above the image, so the first row knows whether it is lit or shaded.
        for (var row = -1; row < size; row++)
        {
            for (var column = 0; column < size; column++)
            {
                var blockX = x + (column - size / 2) * scale;
                var blockZ = z + (row - size / 2) * scale;

                var (block, height, depth) = Top(realm.World, blockX, blockZ, realm.Ceiling(blockX, blockZ));

                if (row >= 0)
                {
                    var color = Color(block, depth);
                    var shade = depth > 0 ? 1.0 : height > previous[column] ? 1.12 : height < previous[column] ? 0.82 : 1.0;

                    var index = (row * size + column) * 3;

                    pixels[index] = (byte) Math.Min(255, color.Red * shade);
                    pixels[index + 1] = (byte) Math.Min(255, color.Green * shade);
                    pixels[index + 2] = (byte) Math.Min(255, color.Blue * shade);
                }

                previous[column] = height;
            }
        }

        return Encode(pixels, size, size);
    }

    private static (Block Block, int Height, int Depth) Top(IWorld world, int x, int z, int ceiling)
    {
        for (var y = ceiling; y > 0; y--)
        {
            var block = world[x, y, z];

            if (block.Type is 0)
            {
                continue;
            }

            if (block.Type is not (8 or 9))
            {
                return (block, y, 0);
            }

            var depth = 0;

            while (y - depth > 0 && world[x, y - depth, z].Type is 8 or 9)
            {
                depth++;
            }

            return (block, y, depth);
        }

        return (Blocks.Air, 0, 0);
    }

    private static (double Red, double Green, double Blue) Color(Block block, int depth)
    {
        if (depth > 0)
        {
            // Deeper water is darker.
            var light = Math.Max(0.45, 1 - depth / 28.0);
            return (48 * light, 92 * light, 210 * light);
        }

        return block.Type switch
        {
            2 or 31 or 175 => (106, 170, 64),
            3 => (134, 96, 67),
            12 => (219, 207, 163),
            24 => (204, 190, 140),
            13 => (136, 126, 126),
            82 => (160, 166, 179),
            1 or 4 => (125, 125, 125),
            17 or 162 => (102, 81, 51),
            18 when block.Metadata is 1 => (61, 99, 61),
            18 when block.Metadata is 2 => (128, 167, 85),
            18 => (59, 122, 38),
            161 => (120, 148, 40),
            78 or 80 => (250, 250, 250),
            79 => (160, 190, 255),
            81 => (13, 99, 25),
            32 => (148, 112, 60),
            83 => (148, 192, 101),
            86 => (227, 144, 29),
            37 or 38 or 39 or 40 => (226, 70, 70),
            95 => (154, 72, 210),
            89 => (250, 220, 120),
            _ => (170, 110, 220)
        };
    }

    private static byte[] Encode(byte[] pixels, int width, int height)
    {
        using var stream = new MemoryStream();

        stream.Write([0x89, (byte) 'P', (byte) 'N', (byte) 'G', 0x0D, 0x0A, 0x1A, 0x0A]);

        var header = new byte[13];

        BinaryPrimitives.WriteInt32BigEndian(header, width);
        BinaryPrimitives.WriteInt32BigEndian(header.AsSpan(4), height);

        header[8] = 8; // Bits per channel.
        header[9] = 2; // Truecolor.

        Chunk(stream, "IHDR", header);

        using var compressed = new MemoryStream();

        using (var zlib = new ZLibStream(compressed, CompressionLevel.Optimal, true))
        {
            for (var row = 0; row < height; row++)
            {
                // Every scanline starts with its filter, none in this case.
                zlib.WriteByte(0);
                zlib.Write(pixels, row * width * 3, width * 3);
            }
        }

        Chunk(stream, "IDAT", compressed.ToArray());
        Chunk(stream, "IEND", []);

        return stream.ToArray();
    }

    private static void Chunk(Stream stream, string type, byte[] data)
    {
        Span<byte> integer = stackalloc byte[4];

        BinaryPrimitives.WriteInt32BigEndian(integer, data.Length);
        stream.Write(integer);

        var name = Encoding.ASCII.GetBytes(type);

        stream.Write(name);
        stream.Write(data);

        var crc = Crc(Crc(uint.MaxValue, name), data) ^ uint.MaxValue;

        BinaryPrimitives.WriteUInt32BigEndian(integer, crc);
        stream.Write(integer);
    }

    private static uint Crc(uint crc, byte[] data)
    {
        foreach (var value in data)
        {
            crc = table[(crc ^ value) & 0xFF] ^ crc >> 8;
        }

        return crc;
    }

    private static uint[] CreateTable()
    {
        var result = new uint[256];

        for (var index = 0u; index < result.Length; index++)
        {
            var value = index;

            for (var bit = 0; bit < 8; bit++)
            {
                value = (value & 1) is 1 ? 0xEDB88320 ^ value >> 1 : value >> 1;
            }

            result[index] = value;
        }

        return result;
    }
}
