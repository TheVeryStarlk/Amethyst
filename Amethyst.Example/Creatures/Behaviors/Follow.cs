using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Extensions;

namespace Amethyst.Example.Creatures.Behaviors;

/// <summary>
/// Keeps a pet at its owner's heels, or circling above their head if it flies.
/// </summary>
internal sealed class Follow(ISpawnable pet, IPlayer owner, Realm realm, bool flying) : IBehavior
{
    private long age;

    public bool Step()
    {
        if (!pet.Tracked())
        {
            return false;
        }

        // Pets go home with their owner.
        if (!owner.World.Players.TryGetValue(owner.Username, out var current) || current != owner)
        {
            pet.Despawn();
            return false;
        }

        age++;

        var position = pet.Position;
        var target = owner.Position;

        var x = target.X - position.X;
        var z = target.Z - position.Z;
        var distance = Math.Sqrt(x * x + z * z);

        // Catch up when left far behind, like after a blink or a teleport.
        if (distance > 24)
        {
            pet.Teleport(new Position(target.X - 1.5, Height(target.X - 1.5, target.Z), target.Z));
            return true;
        }

        var close = distance < 2.5;

        if (pet is IWolf wolf && wolf.IsBegging != close)
        {
            wolf.IsBegging = close;
            wolf.Refresh();
        }

        if (close)
        {
            // Look up at the owner, without flooding them with packets.
            if (age % 4 == 0)
            {
                var y = target.Y + Motion.Eyes - position.Y;
                pet.Face(x, z, (float) (-Math.Atan2(y, Math.Max(distance, 0.1)) * 180 / Math.PI));
            }

            return true;
        }

        // Hurry more the further behind.
        var speed = Math.Min(distance - 2, 0.2 + distance * 0.04);

        var nextX = position.X + x / distance * speed;
        var nextZ = position.Z + z / distance * speed;

        pet.Position = new Position(nextX, Height(nextX, nextZ), nextZ);
        pet.Face(x, z);

        return true;
    }

    private double Height(double x, double z)
    {
        if (flying)
        {
            return owner.Position.Y + 2 + Math.Sin(age / 8.0) * 0.4;
        }

        var surface = realm.Surface((int) Math.Floor(x), (int) Math.Floor(z)).Y;

        // Under a roof or in a cave, the owner's feet are a better guess than the surface far above.
        return surface - owner.Position.Y > 3 ? owner.Position.Y : surface;
    }
}
