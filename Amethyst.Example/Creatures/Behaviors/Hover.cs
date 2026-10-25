using Amethyst.Abstractions.Entities;

namespace Amethyst.Example.Creatures.Behaviors;

/// <summary>
/// Drifts between random spots around a home while bobbing, for anything that flies or swims.
/// </summary>
internal sealed class Hover(ISpawnable entity, Position home, double radius, double speed = 0.15) : IBehavior
{
    private Position? target;
    private long age = Random.Shared.Next(100);

    public bool Step()
    {
        if (!entity.Tracked())
        {
            return false;
        }

        age++;

        var position = entity.Position;

        if (target is not { } current || Motion.Distance(position, current) < speed * 2)
        {
            var angle = Random.Shared.NextDouble() * Math.Tau;
            var distance = Random.Shared.NextDouble() * radius;

            target = new Position(home.X + Math.Cos(angle) * distance, home.Y + Random.Shared.NextDouble() * 4 - 2, home.Z + Math.Sin(angle) * distance);
            return true;
        }

        var x = current.X - position.X;
        var y = current.Y - position.Y;
        var z = current.Z - position.Z;
        var left = Math.Sqrt(x * x + y * y + z * z);

        entity.Position = new Position(
            position.X + x / left * speed,
            position.Y + y / left * speed + Math.Sin(age / 6.0) * 0.04,
            position.Z + z / left * speed);

        entity.Face(x, z);

        return true;
    }
}
