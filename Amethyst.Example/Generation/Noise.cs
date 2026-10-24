namespace Amethyst.Example.Generation;

/// <summary>
/// Seeded two-dimensional Perlin noise.
/// </summary>
internal sealed class Noise
{
    private readonly byte[] permutation = new byte[512];

    public Noise(int seed)
    {
        var source = new byte[256];

        for (var index = 0; index < source.Length; index++)
        {
            source[index] = (byte) index;
        }

        new Random(seed).Shuffle(source);

        // Doubled so lookups never have to wrap around.
        for (var index = 0; index < permutation.Length; index++)
        {
            permutation[index] = source[index & 255];
        }
    }

    /// <returns>A value roughly between -1 and 1.</returns>
    public double Sample(double x, double z)
    {
        var floorX = Math.Floor(x);
        var floorZ = Math.Floor(z);

        var cellX = (int) floorX & 255;
        var cellZ = (int) floorZ & 255;

        var localX = x - floorX;
        var localZ = z - floorZ;

        var u = Fade(localX);
        var v = Fade(localZ);

        var left = permutation[cellX] + cellZ;
        var right = permutation[cellX + 1] + cellZ;

        var near = Lerp(u, Gradient(permutation[left], localX, localZ), Gradient(permutation[right], localX - 1, localZ));
        var far = Lerp(u, Gradient(permutation[left + 1], localX, localZ - 1), Gradient(permutation[right + 1], localX - 1, localZ - 1));

        return Lerp(v, near, far);
    }

    /// <summary>
    /// Layers octaves of noise on top of each other, each one finer and quieter than the last.
    /// </summary>
    /// <returns>A value roughly between -1 and 1.</returns>
    public double Fractal(double x, double z, int octaves)
    {
        var total = 0.0;
        var amplitude = 1.0;
        var frequency = 1.0;
        var maximum = 0.0;

        for (var octave = 0; octave < octaves; octave++)
        {
            // Offset every octave so their lattices do not line up.
            total += Sample(x * frequency + octave * 31.7, z * frequency - octave * 17.3) * amplitude;
            maximum += amplitude;

            amplitude *= 0.5;
            frequency *= 2;
        }

        return total / maximum;
    }

    private static double Fade(double value)
    {
        return value * value * value * (value * (value * 6 - 15) + 10);
    }

    private static double Lerp(double amount, double from, double to)
    {
        return from + amount * (to - from);
    }

    private static double Gradient(int hash, double x, double z)
    {
        return (hash & 7) switch
        {
            0 => x + z,
            1 => -x + z,
            2 => x - z,
            3 => -x - z,
            4 => x,
            5 => -x,
            6 => z,
            _ => -z
        };
    }
}
