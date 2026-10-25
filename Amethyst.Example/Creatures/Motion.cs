using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Abstractions.Worlds;
using Amethyst.Example.Generation;
using Amethyst.Extensions;

namespace Amethyst.Example.Creatures;

/// <summary>
/// Directions, line of sight and the sounds that go along with moving things.
/// </summary>
internal static class Motion
{
    /// <summary>
    /// How high a player's eyes are above their feet.
    /// </summary>
    public const double Eyes = 1.62;

    /// <returns>Whether the world still tracks the entity, it stops once despawned.</returns>
    public static bool Tracked(this ISpawnable entity)
    {
        return entity.World.Entities.ContainsKey(entity.Unique);
    }

    /// <returns>A unit vector pointing where the yaw and pitch, in degrees, look.</returns>
    public static Velocity Direction(double yaw, double pitch)
    {
        yaw *= Math.PI / 180;
        pitch *= Math.PI / 180;

        return new Velocity(-Math.Sin(yaw) * Math.Cos(pitch), -Math.Sin(pitch), Math.Cos(yaw) * Math.Cos(pitch));
    }

    /// <returns>The yaw, in degrees, that looks along the offset.</returns>
    public static float Yaw(double x, double z)
    {
        return (float) (Math.Atan2(-x, z) * 180 / Math.PI);
    }

    public static Position Eye(IPlayer player)
    {
        return player.Position with { Y = player.Position.Y + Eyes };
    }

    public static Position Along(Position position, Velocity direction, double distance)
    {
        return new Position(position.X + direction.X * distance, position.Y + direction.Y * distance, position.Z + direction.Z * distance);
    }

    public static Velocity Scale(Velocity velocity, double factor)
    {
        return new Velocity(velocity.X * factor, velocity.Y * factor, velocity.Z * factor);
    }

    public static double Distance(Position from, Position to)
    {
        var x = to.X - from.X;
        var y = to.Y - from.Y;
        var z = to.Z - from.Z;

        return Math.Sqrt(x * x + y * y + z * z);
    }

    public static bool Solid(IWorld world, Position position)
    {
        // Reading above the world would allocate empty sections.
        return position.Y is >= 0 and < 256 && !Flora.IsPassable(world[(int) Math.Floor(position.X), (int) Math.Floor(position.Y), (int) Math.Floor(position.Z)]);
    }

    /// <returns>The last free spot before the first solid block along the direction, or <see langword="null"/> if nothing is in the way.</returns>
    public static Position? Trace(IWorld world, Position from, Velocity direction, double distance)
    {
        var previous = from;

        for (var traveled = 0.25; traveled <= distance; traveled += 0.25)
        {
            var current = Along(from, direction, traveled);

            if (current.Y is < 0 or >= 256)
            {
                return null;
            }

            if (Solid(world, current))
            {
                return previous;
            }

            previous = current;
        }

        return null;
    }

    /// <summary>
    /// Turns the entity to look along the offset, and shows it to the players in the world.
    /// </summary>
    public static void Face(this ISpawnable entity, double x, double z, float pitch = 0)
    {
        // The ender dragon's model faces backwards.
        var yaw = Yaw(x, z) + (entity is IEnderDragon ? 180 : 0);
        entity.Look(yaw, pitch);
    }

    /// <summary>
    /// Shows the players in the world the entity's metadata, without resending its equipment like <see cref="EntityExtensions.Update"/>.
    /// </summary>
    public static void Refresh(this ISpawnable entity)
    {
        var packet = new EntityMetadataPacket(entity);

        foreach (var pair in entity.World.Players)
        {
            pair.Value.Client.Write(packet);
        }
    }

    public static void Sound(IWorld world, string name, Position position, float volume = 1F, float pitch = 1F)
    {
        var packet = new SoundPacket(name, position, volume, pitch);

        foreach (var pair in world.Players)
        {
            pair.Value.Client.Write(packet);
        }
    }

    public static void Effect(IWorld world, int effect, Position position, int metadata = 0)
    {
        var packet = new EffectPacket(effect, position, metadata);

        foreach (var pair in world.Players)
        {
            pair.Value.Client.Write(packet);
        }
    }
}
