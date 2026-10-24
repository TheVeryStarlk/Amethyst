using Amethyst.Abstractions.Worlds;

namespace Amethyst.Example.Generation;

internal enum TreeKind
{
    Oak,
    Birch,
    Spruce,
    Acacia,
    Elder,
    Amethyst
}

/// <summary>
/// Somewhere trees can be drawn, using world coordinates.
/// </summary>
internal interface ICanvas
{
    public Block this[int x, int y, int z] { get; set; }
}

internal static class Trees
{
    /// <summary>
    /// The farthest any tree reaches horizontally from its trunk.
    /// </summary>
    public const int Reach = 4;

    /// <returns>The chance of a tree growing in a five by five area of the biome.</returns>
    public static double Density(Biome biome)
    {
        return biome switch
        {
            Biome.Forest or Biome.BirchForest => 0.8,
            Biome.Taiga => 0.7,
            Biome.ColdTaiga => 0.45,
            Biome.ExtremeHills => 0.15,
            Biome.Savanna => 0.08,
            Biome.Plains => 0.025,
            Biome.IcePlains => 0.02,
            _ => 0
        };
    }

    public static TreeKind Pick(Biome biome, Random random)
    {
        var roll = random.NextDouble();

        return biome switch
        {
            Biome.Forest => roll switch
            {
                < 0.005 => TreeKind.Amethyst,
                < 0.05 => TreeKind.Elder,
                < 0.25 => TreeKind.Birch,
                _ => TreeKind.Oak
            },
            Biome.BirchForest => roll < 0.005 ? TreeKind.Amethyst : roll < 0.85 ? TreeKind.Birch : TreeKind.Oak,
            Biome.Plains => roll < 0.15 ? TreeKind.Elder : TreeKind.Oak,
            Biome.Savanna => TreeKind.Acacia,
            Biome.ExtremeHills => roll < 0.5 ? TreeKind.Spruce : TreeKind.Oak,
            Biome.Taiga or Biome.ColdTaiga or Biome.IcePlains or Biome.ColdBeach or Biome.FrozenRiver => TreeKind.Spruce,
            Biome.Desert or Biome.Beach => TreeKind.Acacia,
            _ => TreeKind.Oak
        };
    }

    /// <summary>
    /// Draws a tree whose trunk starts at the given position, right above the ground.
    /// </summary>
    public static void Grow(ICanvas canvas, TreeKind kind, int x, int y, int z, Random random)
    {
        // Leave room for the tallest canopy.
        if (y is < 1 or > 240)
        {
            return;
        }

        switch (kind)
        {
            case TreeKind.Oak:
                Round(canvas, x, y, z, 4 + random.Next(3), Blocks.Wood, Blocks.Leaves, random);
                break;

            case TreeKind.Birch:
                Round(canvas, x, y, z, 5 + random.Next(3), new Block(17, 2), new Block(18, 2), random);
                break;

            case TreeKind.Spruce:
                Spruce(canvas, x, y, z, random);
                break;

            case TreeKind.Acacia:
                Acacia(canvas, x, y, z, random);
                break;

            case TreeKind.Elder:
                Elder(canvas, x, y, z, random);
                break;

            case TreeKind.Amethyst:
                Amethyst(canvas, x, y, z, random);
                break;
        }
    }

    // The classic oak and birch shape: two wide layers topped by two narrow ones.
    private static void Round(ICanvas canvas, int x, int y, int z, int height, Block log, Block leaves, Random random)
    {
        var top = y + height;

        for (var layer = top - 3; layer <= top; layer++)
        {
            var radius = layer >= top - 1 ? 1 : 2;

            for (var offsetX = -radius; offsetX <= radius; offsetX++)
            {
                for (var offsetZ = -radius; offsetZ <= radius; offsetZ++)
                {
                    var corner = Math.Abs(offsetX) == radius && Math.Abs(offsetZ) == radius;

                    // Corners are always cut from the top and sometimes from the rest.
                    if (corner && (layer == top || random.Next(2) == 0))
                    {
                        continue;
                    }

                    Leaf(canvas, x + offsetX, layer, z + offsetZ, leaves);
                }
            }
        }

        Trunk(canvas, x, y, z, height, log);
    }

    // Stacked rings that widen towards the bottom, like a cone.
    private static void Spruce(ICanvas canvas, int x, int y, int z, Random random)
    {
        var height = 6 + random.Next(4);
        var top = y + height;
        var bottom = y + 1 + random.Next(2);

        var leaves = new Block(18, 1);

        for (var layer = top; layer >= bottom; layer--)
        {
            var depth = top - layer;
            var widest = Math.Min(3, 1 + depth / 3);
            var radius = depth == 0 ? 0 : depth % 2 == 0 ? widest - 1 : widest;

            for (var offsetX = -radius; offsetX <= radius; offsetX++)
            {
                for (var offsetZ = -radius; offsetZ <= radius; offsetZ++)
                {
                    if (radius > 0 && Math.Abs(offsetX) + Math.Abs(offsetZ) > radius + 1)
                    {
                        continue;
                    }

                    Leaf(canvas, x + offsetX, layer, z + offsetZ, leaves);
                }
            }
        }

        Trunk(canvas, x, y, z, height, new Block(17, 1));
    }

    // A trunk that leans to one side before opening into a flat umbrella canopy.
    private static void Acacia(ICanvas canvas, int x, int y, int z, Random random)
    {
        var straight = 2 + random.Next(2);
        var (stepX, stepZ) = random.Next(4) switch
        {
            0 => (1, 0),
            1 => (-1, 0),
            2 => (0, 1),
            _ => (0, -1)
        };

        Trunk(canvas, x, y, z, straight, Flora.AcaciaLog);

        // One diagonal step keeps the canopy within reach.
        var crownX = x + stepX;
        var crownZ = z + stepZ;
        var crownY = y + straight + 1;

        Log(canvas, crownX, crownY - 1, crownZ, Flora.AcaciaLog);
        Log(canvas, crownX, crownY, crownZ, Flora.AcaciaLog);

        for (var offsetX = -3; offsetX <= 3; offsetX++)
        {
            for (var offsetZ = -3; offsetZ <= 3; offsetZ++)
            {
                var distance = Math.Abs(offsetX) + Math.Abs(offsetZ);

                if (distance <= 4 && !(Math.Abs(offsetX) == 3 && Math.Abs(offsetZ) == 3))
                {
                    Leaf(canvas, crownX + offsetX, crownY + 1, crownZ + offsetZ, Flora.AcaciaLeaves);
                }

                if (distance <= 1)
                {
                    Leaf(canvas, crownX + offsetX, crownY + 2, crownZ + offsetZ, Flora.AcaciaLeaves);
                }
            }
        }
    }

    // A tall, old oak with a billowing canopy and a few branches.
    private static void Elder(ICanvas canvas, int x, int y, int z, Random random)
    {
        var height = 7 + random.Next(3);
        var center = y + height - 1;

        for (var offsetX = -3; offsetX <= 3; offsetX++)
        {
            for (var offsetY = -2; offsetY <= 3; offsetY++)
            {
                for (var offsetZ = -3; offsetZ <= 3; offsetZ++)
                {
                    // Squashed vertically, with a ragged edge.
                    var distance = offsetX * offsetX + offsetY * offsetY * 1.6 + offsetZ * offsetZ;

                    if (distance <= 9.5 || distance <= 12.5 && random.Next(3) > 0)
                    {
                        Leaf(canvas, x + offsetX, center + offsetY, z + offsetZ, Blocks.Leaves);
                    }
                }
            }
        }

        Trunk(canvas, x, y, z, height, Blocks.Wood);

        var branches = 2 + random.Next(2);

        for (var branch = 0; branch < branches; branch++)
        {
            var (stepX, stepZ, axis) = random.Next(4) switch
            {
                0 => (1, 0, 4),
                1 => (-1, 0, 4),
                2 => (0, 1, 8),
                _ => (0, -1, 8)
            };

            var level = center - 2 + random.Next(2);

            // The fourth and eighth metadata bits lay the log along the X and Z axes.
            for (var length = 1; length <= 2; length++)
            {
                Log(canvas, x + stepX * length, level, z + stepZ * length, new Block(17, axis));
            }
        }
    }

    // Rare and glassy, named after the library.
    private static void Amethyst(ICanvas canvas, int x, int y, int z, Random random)
    {
        var height = 5 + random.Next(2);
        var center = y + height;

        for (var offsetX = -3; offsetX <= 3; offsetX++)
        {
            for (var offsetY = -2; offsetY <= 2; offsetY++)
            {
                for (var offsetZ = -3; offsetZ <= 3; offsetZ++)
                {
                    var distance = offsetX * offsetX + offsetY * offsetY * 1.5 + offsetZ * offsetZ;

                    if (distance > 8)
                    {
                        continue;
                    }

                    // Purple stained glass, speckled with magenta.
                    var shard = random.Next(5) == 0 ? new Block(95, 2) : new Block(95, 10);
                    Leaf(canvas, x + offsetX, center + offsetY, z + offsetZ, shard);
                }
            }
        }

        Trunk(canvas, x, y, z, height, new Block(17, 2));

        // A glowing heart right above the trunk.
        canvas[x, center, z] = Blocks.Glowstone;
    }

    private static void Trunk(ICanvas canvas, int x, int y, int z, int height, Block log)
    {
        for (var level = y; level < y + height; level++)
        {
            Log(canvas, x, level, z, log);
        }
    }

    private static void Log(ICanvas canvas, int x, int y, int z, Block log)
    {
        if (canvas[x, y, z].Type is 0 or 18 or 161 or 31 or 37 or 38 or 78 or 175 or 95)
        {
            canvas[x, y, z] = log;
        }
    }

    private static void Leaf(ICanvas canvas, int x, int y, int z, Block leaves)
    {
        if (canvas[x, y, z].Type is 0)
        {
            canvas[x, y, z] = leaves;
        }
    }
}
