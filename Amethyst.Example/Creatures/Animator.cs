using Amethyst.Example.Creatures.Behaviors;
using Microsoft.Extensions.Logging;

namespace Amethyst.Example.Creatures;

/// <summary>
/// Something that plays out over many ticks, like a sheep wandering around.
/// </summary>
internal interface IBehavior
{
    /// <returns>Whether the behavior wants to keep going.</returns>
    public bool Step();
}

/// <summary>
/// Amethyst runs no AI or physics, so the example steps its own behaviors, twenty times a second.
/// </summary>
internal sealed class Animator(ILogger<Animator> logger)
{
    public int Count
    {
        get
        {
            lock (gate)
            {
                return behaviors.Count;
            }
        }
    }

    private readonly List<IBehavior> behaviors = [];
    private readonly Lock gate = new();

    public void Add(IBehavior behavior)
    {
        lock (gate)
        {
            behaviors.Add(behavior);
        }
    }

    public void After(int ticks, Action action)
    {
        Add(new Delay(ticks, action));
    }

    public void Advance()
    {
        IBehavior[] current;

        // Behaviors may add others while stepping, like a firework scheduling its bang.
        lock (gate)
        {
            current = [.. behaviors];
        }

        foreach (var behavior in current)
        {
            bool alive;

            try
            {
                alive = behavior.Step();
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "A {Name} behavior failed", behavior.GetType().Name);
                alive = false;
            }

            if (alive)
            {
                continue;
            }

            lock (gate)
            {
                behaviors.Remove(behavior);
            }
        }
    }
}
