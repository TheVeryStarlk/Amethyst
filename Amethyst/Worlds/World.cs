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

    // Shared by every world and claimed in one step, so usernames stay unique across worlds even when two players join at once.
    private static readonly ConcurrentDictionary<string, IPlayer> usernames = new(StringComparer.OrdinalIgnoreCase);

    // Vanilla treats usernames case-insensitively, players still keep the casing they logged in with.
    private readonly ConcurrentDictionary<string, IPlayer> players = new(StringComparer.OrdinalIgnoreCase);
    private readonly ConcurrentDictionary<long, IChunk> chunks = [];

    // Chunks still being generated, only touched while holding the gate.
    private readonly Dictionary<long, IChunk> pending = [];
    private readonly Lock gate = new();

    /// <returns>False if a player with the same username, ignoring case, is already in this world.</returns>
    public bool TryAdd(IPlayer player)
    {
        return players.TryAdd(player.Username, player);
    }

    public void Remove(IPlayer player)
    {
        // Only remove this exact player, not another one sharing the username.
        players.TryRemove(new KeyValuePair<string, IPlayer>(player.Username, player));
    }

    /// <summary>
    /// Adds a player joining the server, claiming its username across every world first.
    /// </summary>
    /// <returns>False if a player with the same username, ignoring case, is already in any world.</returns>
    public bool TryJoin(IPlayer player)
    {
        if (!usernames.TryAdd(player.Username, player))
        {
            return false;
        }

        if (players.TryAdd(player.Username, player))
        {
            return true;
        }

        // Cannot happen while the claim is held, but the username must never stay claimed by a player that did not join.
        Release(player);
        return false;
    }

    /// <summary>
    /// Removes a player leaving the server and releases its username.
    /// </summary>
    public void Leave(IPlayer player)
    {
        // Released last, so a new player with the same username never finds this one still in a world.
        Remove(player);
        Release(player);
    }

    private static void Release(IPlayer player)
    {
        // Only release this exact player's claim, not a newer one that claimed the username since.
        usernames.TryRemove(new KeyValuePair<string, IPlayer>(player.Username, player));
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

            // Players load chunks from their own threads, so each chunk must only be generated once.
            lock (gate)
            {
                // The generator may read the chunk it is generating back through the world, the gate allows re-entry.
                if (chunks.TryGetValue(key, out chunk) || pending.TryGetValue(key, out chunk))
                {
                    return chunk;
                }

                chunk = new Chunk(x, z);
                pending[key] = chunk;

                try
                {
                    Generator.Generate(this, chunk, x, z);
                }
                finally
                {
                    pending.Remove(key);
                }

                // Only published once fully generated, so other players never see a half-generated chunk.
                chunks[key] = chunk;

                return chunk;
            }
        }
    }
}