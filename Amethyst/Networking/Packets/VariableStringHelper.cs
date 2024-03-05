using System.Text;

namespace Amethyst.Networking.Packets;

internal static class VariableStringHelper
{
    public static int GetBytesCount(string value)
    {
        var length = Encoding.UTF8.GetByteCount(value);
        return VariableIntegerHelper.GetBytesCount(length) + length;
    }
}