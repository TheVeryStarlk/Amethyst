using System.Globalization;
using System.Numerics;

namespace Amethyst.Protocol.Packets.Play.Players;

internal static class GuidExtensions
{
    public static byte[] ToArray(this Guid guid)
    {
        return BigInteger.Parse(guid.ToString().Replace("-", ""), NumberStyles.HexNumber).ToByteArray(isBigEndian: true);
    }
}