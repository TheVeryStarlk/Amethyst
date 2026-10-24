using Amethyst.Abstractions.Worlds;

namespace Amethyst.Example.Generation;

/// <summary>
/// Blocks that are missing from <see cref="Blocks"/> or need a specific metadata.
/// </summary>
internal static class Flora
{
    public static Block TallGrass => new(31, 1);

    public static Block Fern => new(31, 2);

    public static Block SnowLayer => new(78);

    public static Block BrownMushroom => new(39);

    public static Block RedMushroom => new(40);

    public static Block AcaciaLog => new(162);

    public static Block AcaciaLeaves => new(161);

    public static Block Netherrack => new(87);

    /// <summary>
    /// Double plants only know their kind from the lower half, the upper half is marked by the eighth bit.
    /// </summary>
    public static Block UpperHalf => new(175, 8);

    public static Block Sunflower => new(175, 0);

    public static Block DoubleTallGrass => new(175, 2);

    private static readonly Block[] flowers =
    [
        Blocks.Dandelion,
        Blocks.Poppy,
        new(38, 1), // Blue orchid.
        new(38, 2), // Allium.
        new(38, 3), // Azure bluet.
        new(38, 4), // Red tulip.
        new(38, 5), // Orange tulip.
        new(38, 6), // White tulip.
        new(38, 7), // Pink tulip.
        new(38, 8) // Oxeye daisy.
    ];

    private static readonly Block[] bushes =
    [
        Sunflower,
        new(175, 1), // Lilac.
        DoubleTallGrass,
        new(175, 4), // Rose bush.
        new(175, 5) // Peony.
    ];

    public static Block Flower(Random random)
    {
        return flowers[random.Next(flowers.Length)];
    }

    public static Block Bush(Random random)
    {
        return bushes[random.Next(bushes.Length)];
    }

    /// <summary>
    /// Whether players and trees can pass through the block.
    /// </summary>
    public static bool IsPassable(Block block)
    {
        return block.Type is 0 or 8 or 9 or 31 or 32 or 37 or 38 or 39 or 40 or 50 or 51 or 78 or 83 or 175;
    }
}
