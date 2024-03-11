using System.Text;

namespace Amethyst.Utilities;

internal static class VariableString
{
    public static int GetBytesCount(string value)
    {
        var length = Encoding.UTF8.GetByteCount(value);
        return VariableInteger.GetBytesCount(length) + length;
    }
}