using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Worlds;
using Amethyst.Entities.Players;

namespace Amethyst.Worlds;

internal sealed class World(PlayerRepository playerRepository, string name, WorldType type, Dimension dimension, Difficulty difficulty, IGenerator generator) : IWorld
{
    public string Name => name;

    public WorldType Type => type;

    public Dimension Dimension => dimension;

    public Difficulty Difficulty { get; set; } = difficulty;

    public IGenerator Generator => generator;

    public IReadOnlyDictionary<string, IPlayer> Players => playerRepository.Players.Where(pair => pair.Value.World == this).ToDictionary();

    public Block this[Position position]
    {
        get => this[(int) position.X, (int) position.Y, (int) position.Z];
        set => this[(int) position.X, (int) position.Y, (int) position.Z] = value;
    }

    private readonly Dictionary<long, IChunk> chunks = [];

    public Block this[int x, int y, int z]
    {
        get => this[x.ToChunk(), z.ToChunk()][x, y, z];
        set => this[x.ToChunk(), z.ToChunk()][x, y, z] = value;
    }

    public IChunk this[int x, int z]
    {
        get
        {
            var key = NumericUtility.Encode(x, z);

            if (chunks.TryGetValue(key, out var chunk))
            {
                return chunk;
            }

            chunk = new Chunk(x, z);
            chunks[key] = chunk;

            Generator.Generate(this, chunk, x, z);

            return chunk;
        }
    }
}