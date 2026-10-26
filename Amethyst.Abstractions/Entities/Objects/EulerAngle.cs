namespace Amethyst.Abstractions.Entities.Objects;

/// <summary>
/// A rotation in degrees around each axis, used to pose armor stands.
/// </summary>
public struct EulerAngle(float x, float y, float z)
{
    public float X { get; set; } = x;

    public float Y { get; set; } = y;

    public float Z { get; set; } = z;
}