namespace Amethyst.Example.Time;

/// <summary>
/// The shared time of day and weather, advanced by the <see cref="ClockService"/>.
/// </summary>
internal sealed class Clock
{
    /// <summary>
    /// The length of a day, in ticks.
    /// </summary>
    public const long Day = 24000;

    private readonly Lock gate = new();

    private long age;
    private long time = 1000;

    public long Age
    {
        get
        {
            lock (gate)
            {
                return age;
            }
        }
    }

    /// <remarks>Ranges from 0 (sunrise) to <see cref="Day"/>.</remarks>
    public long Time
    {
        get
        {
            lock (gate)
            {
                return time;
            }
        }
        set
        {
            lock (gate)
            {
                time = (value % Day + Day) % Day;
            }
        }
    }

    public bool Raining { get; set; }

    public void Advance(long ticks)
    {
        lock (gate)
        {
            age += ticks;
            time = (time + ticks) % Day;
        }
    }

    /// <summary>
    /// The time of day as a 24-hour clock, the game's day starts at six in the morning.
    /// </summary>
    public string Display()
    {
        var current = Time;

        var hours = (current / 1000 + 6) % 24;
        var minutes = current % 1000 * 60 / 1000;

        return $"{hours:00}:{minutes:00}";
    }
}
