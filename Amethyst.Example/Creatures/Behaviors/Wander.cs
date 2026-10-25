using Amethyst.Abstractions.Entities;
using Amethyst.Extensions;

namespace Amethyst.Example.Creatures.Behaviors;

/// <summary>
/// Strolls between random spots around a home, resting in between, and falls when nothing is below.
/// </summary>
internal sealed class Wander(ISpawnable entity, Realm realm, Position home, double radius, double speed = 0.12) : IBehavior
{
    private (double X, double Z)? target;
    private int rest = Random.Shared.Next(20, 100);
    private double fall;

    public bool Step()
    {
        if (!entity.Tracked())
        {
            return false;
        }

        var position = entity.Position;
        var ground = Ground(position.X, position.Z);

        // Nothing holds entities up but this.
        if (position.Y > ground + 0.01)
        {
            fall = (fall - 0.08) * 0.98;
            entity.Teleport(position with { Y = Math.Max(ground, position.Y + fall) });

            return true;
        }

        fall = 0;

        if (rest > 0)
        {
            rest--;

            // Glance around every now and then.
            if (Random.Shared.Next(80) == 0)
            {
                entity.Look(entity.Yaw + Random.Shared.Next(-70, 71), Random.Shared.Next(-20, 21));
            }

            return true;
        }

        if (target is not { } current)
        {
            var angle = Random.Shared.NextDouble() * Math.Tau;
            var distance = Random.Shared.NextDouble() * radius;

            target = (home.X + Math.Cos(angle) * distance, home.Z + Math.Sin(angle) * distance);
            return true;
        }

        var x = current.X - position.X;
        var z = current.Z - position.Z;
        var left = Math.Sqrt(x * x + z * z);

        if (left < speed)
        {
            Settle(40, 200);
            return true;
        }

        var next = new Position(position.X + x / left * speed, position.Y, position.Z + z / left * speed);
        var height = Ground(next.X, next.Z);

        // Turn back at cliffs and water rather than climbing or swimming.
        if (height - position.Y > 1.1 || realm.World[(int) Math.Floor(next.X), (int) height - 1, (int) Math.Floor(next.Z)].Type is 8 or 9)
        {
            Settle(10, 40);
            return true;
        }

        // Stepping down is left to falling.
        entity.Position = next with { Y = Math.Max(height, position.Y) };
        entity.Face(x, z);

        return true;
    }

    private void Settle(int minimum, int maximum)
    {
        target = null;
        rest = Random.Shared.Next(minimum, maximum);
    }

    private double Ground(double x, double z)
    {
        return realm.Surface((int) Math.Floor(x), (int) Math.Floor(z)).Y;
    }
}
