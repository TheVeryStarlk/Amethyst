using Amethyst.Abstractions.Worlds;

namespace Amethyst.Example.Generation;

/// <summary>
/// The terrain's height and biome at a single column, independent of any generated chunk.
/// </summary>
internal readonly record struct Column(int Height, Biome Biome);

/// <summary>
/// Generates oceans, beaches, rivers, plains, forests, deserts, savannas, taigas and snowy mountains,
/// decorated with ores, flowers and trees.
/// </summary>
internal sealed class OverworldGenerator(int seed) : IGenerator
{
    public const int SeaLevel = 62;

    /// <summary>
    /// Every biome this generator produces.
    /// </summary>
    public static Biome[] Biomes { get; } =
    [
        Biome.Ocean,
        Biome.DeepOcean,
        Biome.FrozenOcean,
        Biome.Beach,
        Biome.ColdBeach,
        Biome.River,
        Biome.FrozenRiver,
        Biome.Plains,
        Biome.Forest,
        Biome.BirchForest,
        Biome.Desert,
        Biome.Savanna,
        Biome.Taiga,
        Biome.ColdTaiga,
        Biome.IcePlains,
        Biome.ExtremeHills
    ];

    public int Seed => seed;

    // Trees are scattered on a jittered grid, so neighbouring trunks are never too close.
    private const int Spacing = 5;

    private static readonly (Block Ore, int Veins, int Size, int Ceiling)[] ores =
    [
        (Blocks.CoalOre, 16, 10, 128),
        (Blocks.IronOre, 12, 7, 64),
        (Blocks.GoldOre, 3, 6, 32),
        (Blocks.RedstoneOre, 6, 6, 16),
        (Blocks.LapisLazuliOre, 2, 5, 32),
        (Blocks.DiamondOre, 1, 5, 16)
    ];

    private readonly Noise continents = new(seed);
    private readonly Noise hills = new(seed + 1);
    private readonly Noise ridges = new(seed + 2);
    private readonly Noise rivers = new(seed + 3);
    private readonly Noise temperature = new(seed + 4);
    private readonly Noise humidity = new(seed + 5);
    private readonly Noise patches = new(seed + 6);

    public Column Sample(int x, int z)
    {
        var continent = continents.Fractal(x / 640.0, z / 640.0, 4);
        var inland = SmoothStep(-0.1, 0.25, continent);

        var height = SeaLevel + 4 + continent * 40 + hills.Fractal(x / 128.0, z / 128.0, 4) * (5 + 16 * inland);

        // Ridged noise peaks where it crosses zero, but mountains only rise away from the coast.
        var ridge = 1 - Math.Abs(ridges.Fractal(x / 300.0, z / 300.0, 5));
        height += Math.Pow(ridge, 5) * 95 * SmoothStep(0.0, 0.3, continent);

        // Rivers also follow the noise's zero crossings, carving valleys down to below the sea level.
        var carving = 1 - SmoothStep(0.0, 0.045, Math.Abs(rivers.Fractal(x / 480.0, z / 480.0, 3)));
        var river = carving > 0.55 && height > SeaLevel;

        if (height > SeaLevel - 4)
        {
            height = Lerp(carving, height, SeaLevel - 4);
        }

        var level = Math.Clamp((int) Math.Round(height), 4, 220);

        var heat = temperature.Fractal(x / 1024.0, z / 1024.0, 3) * 2 - (level - SeaLevel) / 120.0;
        var moisture = humidity.Fractal(x / 768.0, z / 768.0, 3) * 2;

        var cold = heat < -0.5;

        var biome = level switch
        {
            _ when river => cold ? Biome.FrozenRiver : Biome.River,
            < SeaLevel - 18 => cold ? Biome.FrozenOcean : Biome.DeepOcean,
            < SeaLevel => cold ? Biome.FrozenOcean : Biome.Ocean,
            <= SeaLevel + 1 when continent < 0.0 => cold ? Biome.ColdBeach : Biome.Beach,
            > SeaLevel + 48 => Biome.ExtremeHills,
            _ when heat > 0.25 => moisture < 0.1 ? Biome.Desert : Biome.Savanna,
            _ when cold => moisture > -0.1 ? Biome.ColdTaiga : Biome.IcePlains,
            _ when heat < -0.25 => Biome.Taiga,
            _ when moisture > 0.35 => Biome.BirchForest,
            _ when moisture > 0.0 => Biome.Forest,
            _ => Biome.Plains
        };

        return new Column(level, biome);
    }

    public void Generate(IWorld world, IChunk chunk, int x, int z)
    {
        var columns = new Column[256];

        for (var localX = 0; localX < 16; localX++)
        {
            for (var localZ = 0; localZ < 16; localZ++)
            {
                var column = Sample(x * 16 + localX, z * 16 + localZ);

                columns[localZ * 16 + localX] = column;
                chunk[localX, localZ] = column.Biome;

                Fill(chunk, localX, localZ, x * 16 + localX, z * 16 + localZ, column);
            }
        }

        Mine(chunk, x, z);
        Plant(chunk, x, z);
        Decorate(chunk, x, z, columns);
    }

    /// <summary>
    /// The block covering the ground of a column.
    /// </summary>
    public Block Surface(int x, int z, Column column)
    {
        if (column.Height < SeaLevel)
        {
            // Patches of gravel and clay break up the sandy sea floor.
            var patch = patches.Sample(x / 12.0, z / 12.0);
            return patch > 0.35 ? Blocks.Gravel : patch < -0.45 ? Blocks.Clay : Blocks.Sand;
        }

        return column.Biome switch
        {
            Biome.Desert or Biome.Beach or Biome.ColdBeach => Blocks.Sand,
            Biome.River or Biome.FrozenRiver => Blocks.Sand,
            Biome.ExtremeHills when column.Height > SeaLevel + 64 => Blocks.Stone,
            _ => Blocks.GrassBlock
        };
    }

    private void Fill(IChunk chunk, int localX, int localZ, int x, int z, Column column)
    {
        chunk[localX, 0, localZ] = Blocks.Bedrock;

        var surface = Surface(x, z, column);

        // Sand is carried by sandstone, everything else by dirt.
        var filler = surface.Type == Blocks.Sand.Type ? Blocks.Sandstone : surface.Type == Blocks.Stone.Type ? Blocks.Stone : Blocks.Dirt;
        var depth = column.Biome is Biome.Desert ? 6 : 4;

        for (var y = 1; y <= column.Height; y++)
        {
            // A rough bedrock floor that thins out as it rises.
            if (y < 5 && Scatter.Chance(seed, x, z, y) < 1 - y * 0.2)
            {
                chunk[localX, y, localZ] = Blocks.Bedrock;
                continue;
            }

            var below = column.Height - y;

            chunk[localX, y, localZ] = below switch
            {
                0 => surface,
                < 3 when surface.Type == Blocks.Sand.Type => Blocks.Sand,
                _ when below < depth => filler,
                _ => Blocks.Stone
            };
        }

        for (var y = column.Height + 1; y <= SeaLevel; y++)
        {
            chunk[localX, y, localZ] = Blocks.StationaryWater;
        }

        var frozen = column.Biome is Biome.FrozenOcean or Biome.FrozenRiver or Biome.ColdBeach or Biome.ColdTaiga or Biome.IcePlains;

        if (column.Height < SeaLevel)
        {
            if (frozen)
            {
                chunk[localX, SeaLevel, localZ] = Blocks.Ice;
            }

            return;
        }

        if (frozen || column.Height > SeaLevel + 72)
        {
            chunk[localX, column.Height + 1, localZ] = Flora.SnowLayer;
        }
    }

    // Ore veins wander randomly through the stone, cut off at the chunk's edges.
    private void Mine(IChunk chunk, int x, int z)
    {
        var random = new Random(Scatter.Hash(seed, x, z, 7));

        foreach (var (ore, veins, size, ceiling) in ores)
        {
            for (var vein = 0; vein < veins; vein++)
            {
                var localX = random.Next(16);
                var y = random.Next(1, ceiling);
                var localZ = random.Next(16);

                for (var step = 0; step < size; step++)
                {
                    if (localX is >= 0 and < 16 && localZ is >= 0 and < 16 && y > 0 && chunk[localX, y, localZ].Type == Blocks.Stone.Type)
                    {
                        chunk[localX, y, localZ] = ore;
                    }

                    switch (random.Next(3))
                    {
                        case 0:
                            localX += random.Next(2) * 2 - 1;
                            break;

                        case 1:
                            y += random.Next(2) * 2 - 1;
                            break;

                        default:
                            localZ += random.Next(2) * 2 - 1;
                            break;
                    }
                }
            }
        }
    }

    // Every tree that could reach into this chunk is drawn, but only the blocks inside of it are kept.
    // Trees are visited in the same order from every chunk, so overlapping trees always look the same.
    private void Plant(IChunk chunk, int x, int z)
    {
        var canvas = new ChunkCanvas(chunk, x, z);

        var minimumX = FloorDivide(x * 16 - Trees.Reach, Spacing);
        var maximumX = FloorDivide(x * 16 + 15 + Trees.Reach, Spacing);
        var minimumZ = FloorDivide(z * 16 - Trees.Reach, Spacing);
        var maximumZ = FloorDivide(z * 16 + 15 + Trees.Reach, Spacing);

        for (var cellX = minimumX; cellX <= maximumX; cellX++)
        {
            for (var cellZ = minimumZ; cellZ <= maximumZ; cellZ++)
            {
                var random = new Random(Scatter.Hash(seed, cellX, cellZ, 3));

                var trunkX = cellX * Spacing + 1 + random.Next(Spacing - 2);
                var trunkZ = cellZ * Spacing + 1 + random.Next(Spacing - 2);

                var column = Sample(trunkX, trunkZ);

                if (random.NextDouble() >= Trees.Density(column.Biome) || Surface(trunkX, trunkZ, column).Type != Blocks.GrassBlock.Type)
                {
                    continue;
                }

                Trees.Grow(canvas, Trees.Pick(column.Biome, random), trunkX, column.Height + 1, trunkZ, random);
            }
        }
    }

    private void Decorate(IChunk chunk, int x, int z, Column[] columns)
    {
        var random = new Random(Scatter.Hash(seed, x, z, 11));

        for (var localX = 0; localX < 16; localX++)
        {
            for (var localZ = 0; localZ < 16; localZ++)
            {
                var column = columns[localZ * 16 + localX];
                var y = column.Height + 1;

                // Rolled for every column, so the outcome does not shift when a single column changes.
                var roll = random.NextDouble();

                if (column.Height < SeaLevel || y > 252 || chunk[localX, y, localZ].Type != 0)
                {
                    continue;
                }

                var ground = chunk[localX, column.Height, localZ];

                if (ground.Type == Blocks.Sand.Type || ground.Type == Blocks.GrassBlock.Type)
                {
                    // Sugar cane grows along any shore.
                    if (roll < 0.06 && column.Height <= SeaLevel + 1 && Shore(x * 16 + localX, z * 16 + localZ))
                    {
                        Stack(chunk, localX, y, localZ, Blocks.SugarCane, 1 + random.Next(3));
                        continue;
                    }
                }

                if (ground.Type == Blocks.Sand.Type && column.Biome is Biome.Desert)
                {
                    if (roll < 0.006)
                    {
                        Stack(chunk, localX, y, localZ, Blocks.Cactus, 1 + random.Next(3));
                    }
                    else if (roll < 0.014)
                    {
                        chunk[localX, y, localZ] = Blocks.DeadBush;
                    }

                    continue;
                }

                if (ground.Type != Blocks.GrassBlock.Type)
                {
                    continue;
                }

                switch (column.Biome)
                {
                    case Biome.Plains:
                        if (roll < 0.002)
                        {
                            chunk[localX, y, localZ] = new Block(Blocks.Pumpkin.Type, random.Next(4));
                        }
                        else if (roll < 0.006)
                        {
                            Double(chunk, localX, y, localZ, Flora.Sunflower);
                        }
                        else if (roll < 0.03)
                        {
                            chunk[localX, y, localZ] = Flora.Flower(random);
                        }
                        else if (roll < 0.2)
                        {
                            chunk[localX, y, localZ] = Flora.TallGrass;
                        }

                        break;

                    case Biome.Forest or Biome.BirchForest:
                        if (roll < 0.004)
                        {
                            chunk[localX, y, localZ] = random.Next(2) == 0 ? Flora.BrownMushroom : Flora.RedMushroom;
                        }
                        else if (roll < 0.008)
                        {
                            Double(chunk, localX, y, localZ, Flora.Bush(random));
                        }
                        else if (roll < 0.02)
                        {
                            chunk[localX, y, localZ] = Flora.Flower(random);
                        }
                        else if (roll < 0.1)
                        {
                            chunk[localX, y, localZ] = Flora.TallGrass;
                        }

                        break;

                    case Biome.Taiga:
                        if (roll < 0.12)
                        {
                            chunk[localX, y, localZ] = Flora.Fern;
                        }

                        break;

                    case Biome.Savanna:
                        if (roll < 0.01)
                        {
                            Double(chunk, localX, y, localZ, Flora.DoubleTallGrass);
                        }
                        else if (roll < 0.3)
                        {
                            chunk[localX, y, localZ] = Flora.TallGrass;
                        }

                        break;

                    case Biome.ExtremeHills:
                        if (roll < 0.05)
                        {
                            chunk[localX, y, localZ] = Flora.TallGrass;
                        }

                        break;
                }
            }
        }
    }

    private bool Shore(int x, int z)
    {
        return Sample(x + 1, z).Height < SeaLevel
               || Sample(x - 1, z).Height < SeaLevel
               || Sample(x, z + 1).Height < SeaLevel
               || Sample(x, z - 1).Height < SeaLevel;
    }

    private static void Stack(IChunk chunk, int localX, int y, int localZ, Block block, int height)
    {
        for (var level = y; level < y + height; level++)
        {
            chunk[localX, level, localZ] = block;
        }
    }

    private static void Double(IChunk chunk, int localX, int y, int localZ, Block lower)
    {
        chunk[localX, y, localZ] = lower;
        chunk[localX, y + 1, localZ] = Flora.UpperHalf;
    }

    private static int FloorDivide(int value, int divisor)
    {
        return (int) Math.Floor(value / (double) divisor);
    }

    private static double SmoothStep(double from, double to, double value)
    {
        var amount = Math.Clamp((value - from) / (to - from), 0, 1);
        return amount * amount * (3 - 2 * amount);
    }

    private static double Lerp(double amount, double from, double to)
    {
        return from + amount * (to - from);
    }

    /// <summary>
    /// Only keeps the blocks that fall inside of the chunk being generated.
    /// </summary>
    private sealed class ChunkCanvas(IChunk chunk, int chunkX, int chunkZ) : ICanvas
    {
        public Block this[int x, int y, int z]
        {
            get => Contains(x, y, z) ? chunk[x - chunkX * 16, y, z - chunkZ * 16] : Blocks.Air;
            set
            {
                if (Contains(x, y, z))
                {
                    chunk[x - chunkX * 16, y, z - chunkZ * 16] = value;
                }
            }
        }

        private bool Contains(int x, int y, int z)
        {
            return x >> 4 == chunkX && z >> 4 == chunkZ && y is >= 0 and < 256;
        }
    }
}
