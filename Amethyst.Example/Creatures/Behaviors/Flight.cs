using Amethyst.Abstractions.Entities;

namespace Amethyst.Example.Creatures.Behaviors;

/// <summary>
/// How a thrown, shot or falling entity moves each tick.
/// </summary>
/// <param name="Acceleration">Pushes fireballs along their direction, they ignore gravity.</param>
internal sealed record Physics(double Gravity, double Drag, double Acceleration = 0)
{
    public static Physics Thrown { get; } = new(0.03, 0.99);

    public static Physics Arrow { get; } = new(0.05, 0.99);

    public static Physics Falling { get; } = new(0.04, 0.98);

    public static Physics Fireball { get; } = new(0, 0.95, 0.1);
}

/// <summary>
/// Follows the physics clients use for the entity on their own, to know when and where it lands.
/// </summary>
/// <remarks>Clients move these entities by themselves, so nothing is sent while they fly.</remarks>
internal sealed class Flight(ISpawnable entity, Physics physics, Action<Position> landed, int lifetime = 300) : IBehavior
{
    private readonly Velocity acceleration = Accelerate(entity.Velocity, physics.Acceleration);

    private Position position = entity.Position;
    private Velocity motion = entity.Velocity;
    private int age;

    public bool Step()
    {
        if (!entity.Tracked())
        {
            return false;
        }

        // Fast entities cross whole blocks in a tick, so look in between too.
        for (var part = 1; part <= 4; part++)
        {
            var next = new Position(
                position.X + motion.X * part / 4,
                position.Y + motion.Y * part / 4,
                position.Z + motion.Z * part / 4);

            if (next.Y < 0 || Motion.Solid(entity.World, next))
            {
                landed(position);
                return false;
            }
        }

        position = new Position(position.X + motion.X, position.Y + motion.Y, position.Z + motion.Z);

        motion = new Velocity(
            (motion.X + acceleration.X) * physics.Drag,
            (motion.Y + acceleration.Y) * physics.Drag - physics.Gravity,
            (motion.Z + acceleration.Z) * physics.Drag);

        if (++age < lifetime)
        {
            return true;
        }

        landed(position);
        return false;
    }

    // Clients scale a fireball's direction to the same acceleration.
    private static Velocity Accelerate(Velocity velocity, double acceleration)
    {
        var length = Math.Sqrt(velocity.X * velocity.X + velocity.Y * velocity.Y + velocity.Z * velocity.Z);

        return length is 0 || acceleration is 0
            ? default
            : Motion.Scale(velocity, acceleration / length);
    }
}
