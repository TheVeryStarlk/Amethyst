using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;
using System.Collections.Concurrent;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Worlds;

internal sealed class World(string name, WorldType type, Dimension dimension, Difficulty difficulty, IGenerator generator) : IWorld
{
    public string Name => name;

    public WorldType Type => type;

    public Dimension Dimension => dimension;

    public Difficulty Difficulty { get; set; } = difficulty;

    public IGenerator Generator => generator;

    public IReadOnlyDictionary<string, IPlayer> Players => players;

    public Block this[Position position]
    {
        get => this[(int) position.X, (int) position.Y, (int) position.Z];
        set => this[(int) position.X, (int) position.Y, (int) position.Z] = value;
    }

    private readonly ConcurrentDictionary<string, IPlayer> players = [];
    private readonly Dictionary<long, IChunk> chunks = [];

    /// <returns>False if a player with the same username is already in this world.</returns>
    public bool TryAdd(IPlayer player)
    {
        return players.TryAdd(player.Username, player);
    }

    public void Remove(IPlayer player)
    {
        // Only remove this exact player, not another one sharing the username.
        players.TryRemove(new KeyValuePair<string, IPlayer>(player.Username, player));
    }

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