using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Worlds;
using Amethyst.Entities;
using Amethyst.Utilities;

namespace Amethyst.Worlds;

internal sealed class World(PlayerRepository playerRepository, string name, WorldType type, Dimension dimension, Difficulty difficulty, IGenerator generator) : IWorld
{
    public string Name => name;

    public WorldType Type => type;

    public Dimension Dimension => dimension;

    public Difficulty Difficulty { get; set; } = difficulty;

    public IGenerator Generator => generator;

    public IReadOnlyDictionary<string, IPlayer> Players => playerRepository.Players.Where(pair => pair.Value.World == this).ToDictionary();

    private readonly Dictionary<long, Chunk> chunks = [];

    public Block this[int x, int y, int z]
    {
        get => this[x.ToChunk(), z.ToChunk()][x, y, z];
        set => this[x.ToChunk(), z.ToChunk()][x, y, z] = value;
    }

    public IChunk this[int x, int z]
    {
        get
        {
            var value = NumericUtility.Encode(x, z);

            if (chunks.TryGetValue(value, out var chunk))
            {
                return chunk;
            }

            chunk = new Chunk(x, z);
            chunks[value] = chunk;

            Generator.Generate(this, chunk, x, z);

            return chunk;
        }
    }
}