using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Worlds;
using Amethyst.Example.Generation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Amethyst.Example;

/// <summary>
/// Owns the overworld that every subscriber shares.
/// </summary>
internal sealed class Realm
{
    public int Seed { get; }

    public OverworldGenerator Generator { get; }

    public IWorld World { get; }

    public Position Spawn { get; }

    /// <summary>
    /// A top-down map of the spawn, shown as the server's icon.
    /// </summary>
    public string Favicon { get; }

    public Realm(ILogger<Realm> logger, IConfiguration configuration, IWorldFactory worldFactory)
    {
        // Pass "--seed 1234" to explore a specific world.
        Seed = configuration.GetValue<int?>("Seed") ?? Random.Shared.Next();
        Generator = new OverworldGenerator(Seed);

        // Create the world once, players are tracked by the world they are in.
        World = worldFactory.Create("Overworld", Generator);

        Spawn = FindSpawn();
        Favicon = "data:image/png;base64," + Convert.ToBase64String(Map.Render(this, (int) Math.Floor(Spawn.X), (int) Math.Floor(Spawn.Z), 64, 2));

        logger.LogInformation("Generated the overworld with seed {Seed}, spawning at {X}, {Y}, {Z}", Seed, Spawn.X, Spawn.Y, Spawn.Z);
    }

    public Biome BiomeAt(Position position)
    {
        return Generator.Sample((int) Math.Floor(position.X), (int) Math.Floor(position.Z)).Biome;
    }

    /// <summary>
    /// The highest a generated column could reach, trees included.
    /// </summary>
    /// <remarks>Reading blocks above it would make the chunk allocate empty sections and send them to players.</remarks>
    public int Ceiling(int x, int z)
    {
        return Math.Min(255, Generator.Sample(x, z).Height + 24);
    }

    /// <returns>The center of the block right above the highest solid block of the column.</returns>
    public Position Surface(int x, int z)
    {
        for (var y = Ceiling(x, z); y > 0; y--)
        {
            if (!Flora.IsPassable(World[x, y, z]) || World[x, y, z].Type is 8 or 9)
            {
                return new Position(x + 0.5, y + 1, z + 0.5);
            }
        }

        return new Position(x + 0.5, OverworldGenerator.SeaLevel + 1, z + 0.5);
    }

    /// <returns>The closest column of the biome, or <see langword="null"/> if there is none nearby.</returns>
    public (int X, int Z)? Find(Biome biome, int x, int z, int radius = 8192)
    {
        foreach (var (columnX, columnZ) in Spiral(x, z, 32, radius))
        {
            if (Generator.Sample(columnX, columnZ).Biome == biome)
            {
                return (columnX, columnZ);
            }
        }

        return null;
    }

    // Prefers open plains, where nobody spawns on top of a tree.
    private Position FindSpawn()
    {
        foreach (var (x, z) in Spiral(0, 0, 8, 4096))
        {
            if (Generator.Sample(x, z).Biome is not Biome.Plains)
            {
                continue;
            }

            var surface = Surface(x, z);

            if (World[x, (int) surface.Y - 1, z].Type == Blocks.GrassBlock.Type)
            {
                return surface;
            }
        }

        return Surface(0, 0);
    }

    /// <summary>
    /// Walks square rings outwards from the center.
    /// </summary>
    private static IEnumerable<(int X, int Z)> Spiral(int x, int z, int step, int radius)
    {
        yield return (x, z);

        for (var ring = step; ring <= radius; ring += step)
        {
            for (var offset = -ring; offset < ring; offset += step)
            {
                yield return (x + offset, z - ring);
                yield return (x + ring, z + offset);
                yield return (x - offset, z + ring);
                yield return (x - ring, z - offset);
            }
        }
    }
}
