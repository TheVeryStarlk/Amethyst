namespace Amethyst.Example.Creatures.Behaviors;

/// <summary>
/// Runs an action once the ticks have passed.
/// </summary>
internal sealed class Delay(int ticks, Action action) : IBehavior
{
    private int left = ticks;

    public bool Step()
    {
        if (--left > 0)
        {
            return true;
        }

        action();
        return false;
    }
}
