using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;

namespace Amethyst.Entities;

// Taken from https://github.com/ObsidianMC/Obsidian/blob/1.21.x/Obsidian/Utilities/GuidHelper.cs with some changes.
internal static class GuidHelper
{
    public static Guid FromString(string text)
    {
        var integer = new BigInteger();

        MD5.HashData(Encoding.UTF8.GetBytes(text), integer.AsSpan());

        integer.Version = (byte) (integer.Version & 0x0F | 0x30);
        integer.Variant = (byte) (integer.Variant & 0x3F | 0x80);

        return Unsafe.As<BigInteger, Guid>(ref integer);
    }

    [StructLayout(LayoutKind.Explicit, Size = 16)]
    private struct BigInteger
    {
        [FieldOffset(0)]
        public int first;

        [FieldOffset(4)]
        public int second;

        [FieldOffset(8)]
        public int third;

        [FieldOffset(12)]
        public int fourth;

        [FieldOffset(0)]
        private byte start;

        [FieldOffset(7)]
        public byte Version;

        [FieldOffset(8)]
        public byte Variant;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Span<byte> AsSpan()
        {
            return MemoryMarshal.CreateSpan(ref start, 16);
        }
    }
}