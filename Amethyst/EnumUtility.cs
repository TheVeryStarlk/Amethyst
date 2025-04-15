namespace Amethyst;

internal static class EnumUtility
{
    public static T Convert<T>(int value) where T : Enum
    {
        if (!Enum.IsDefined(typeof(T), value))
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, "Invalid value.");
        }

        return (T) (object) value;
    }
}