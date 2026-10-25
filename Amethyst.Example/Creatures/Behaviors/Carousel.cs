using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Mobs;

namespace Amethyst.Example.Creatures.Behaviors;

/// <summary>
/// Spins horses around a pole like a merry-go-round, bobbing them up and down to a little tune.
/// </summary>
internal sealed class Carousel(Position center, IHorse[] horses, double radius) : IBehavior
{
    // Semitones above the note block's lowest note, a waltz going round and round.
    private static readonly int[] tune = [12, 16, 19, 16, 12, 16, 19, 24, 21, 17, 21, 17, 14, 17, 19, 23, 24, 19, 16, 19, 12, 16, 19, 16];

    private long age;

    public bool Step()
    {
        if (!horses.Any(horse => horse.Tracked()))
        {
            return false;
        }

        age++;

        for (var index = 0; index < horses.Length; index++)
        {
            var angle = age * 0.03 + index * Math.Tau / horses.Length;

            horses[index].Position = new Position(
                center.X + Math.Cos(angle) * radius,
                center.Y + 0.6 + Math.Sin(age * 0.15 + index * 1.7) * 0.4,
                center.Z + Math.Sin(angle) * radius);

            // Each horse faces along the circle.
            horses[index].Face(-Math.Sin(angle), Math.Cos(angle));
        }

        if (age % 5 == 0)
        {
            var note = tune[age / 5 % tune.Length];
            Motion.Sound(horses[0].World, Sounds.NoteHarp, center, 1.5F, (float) Math.Pow(2, (note - 12) / 12.0));
        }

        return true;
    }
}
