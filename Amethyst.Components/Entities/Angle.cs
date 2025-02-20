namespace Amethyst.Components.Entities;

public readonly record struct Angle(float Value)
{
    public static implicit operator Angle(float value)
    {
        return new Angle(value);
    }

    public static implicit operator float(Angle value)
    {
        return value.Value;
    }

    public byte ToAbsolute()
    {
        return (byte) (Value % 360 / 360 * 256);
    }
}