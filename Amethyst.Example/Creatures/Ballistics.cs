using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Worlds;
using Amethyst.Example.Creatures.Behaviors;
using Amethyst.Example.Subscribers;
using Amethyst.Extensions;

namespace Amethyst.Example.Creatures;

/// <summary>
/// Everything that gets thrown, shot, dropped or blown up.
/// </summary>
/// <remarks>Clients fly these entities on their own, the server follows along to know where they land.</remarks>
internal sealed class Ballistics(Animator animator)
{
    /// <summary>
    /// Throws a snowball, egg, ender pearl, bottle or potion, which breaks wherever it lands.
    /// </summary>
    public T Throw<T>(IWorld world, Position origin, Velocity direction, double speed, Action<T>? configure = null, Action<Position>? landed = null) where T : ISpawnable
    {
        var thrown = world.Spawn<T>(origin, entity =>
        {
            entity.Velocity = Motion.Scale(direction, speed);
            configure?.Invoke(entity);
        });

        animator.Add(new Flight(thrown, Physics.Thrown, landing =>
        {
            thrown.Despawn();
            landed?.Invoke(landing);
        }));

        return thrown;
    }

    /// <summary>
    /// Shoots a critical arrow, which sticks where it lands for a while.
    /// </summary>
    public IArrow Shoot(IWorld world, Position origin, Velocity direction, IEntity? shooter, double speed = 3)
    {
        var arrow = world.Spawn<IArrow>(origin, arrow =>
        {
            arrow.Velocity = Motion.Scale(direction, speed);
            arrow.Shooter = shooter;
            arrow.IsCritical = true;
        });

        animator.Add(new Flight(arrow, Physics.Arrow, _ =>
        {
            Motion.Sound(world, Sounds.RandomBowHit, arrow.Position, 0.6F, 1.2F);
            animator.After(200, arrow.Despawn);
        }));

        return arrow;
    }

    /// <summary>
    /// Hurls a fireball that explodes where it lands, or sets it alight if it has no power.
    /// </summary>
    /// <remarks>Clients only accelerate fireballs with a shooter.</remarks>
    public T Fire<T>(IWorld world, Position origin, Velocity direction, IEntity shooter, double power, Action<T>? configure = null) where T : IFireball
    {
        var fireball = world.Spawn<T>(origin, fireball =>
        {
            fireball.Velocity = direction;
            fireball.Shooter = shooter;
            configure?.Invoke(fireball);
        });

        animator.Add(new Flight(fireball, Physics.Fireball, landing =>
        {
            fireball.Despawn();

            if (power > 0)
            {
                Explode(world, landing, power);
                return;
            }

            Ignite(world, landing);
        }, 200));

        return fireball;
    }

    /// <summary>
    /// Drops a block from the sky, which turns back into a block once it lands.
    /// </summary>
    public IFallingBlock Drop(IWorld world, Position origin, Block block)
    {
        var falling = world.Spawn<IFallingBlock>(origin, falling => falling.Block = block);

        animator.Add(new Flight(falling, Physics.Falling, landing =>
        {
            falling.Despawn();

            var where = new Position(Math.Floor(landing.X), Math.Floor(landing.Y), Math.Floor(landing.Z));
            world.Set(where, block);

            if (block.Type == Blocks.Anvil.Type)
            {
                Motion.Effect(world, Effects.AnvilLand, where);
            }
        }));

        return falling;
    }

    /// <summary>
    /// Launches a firework rocket, which bangs high up in the sky.
    /// </summary>
    public IFirework Firework(IWorld world, Position origin)
    {
        var random = Random.Shared;

        // Clients speed rockets up sideways and upwards on their own.
        var rocket = world.Spawn<IFirework>(origin, rocket =>
        {
            rocket.Item = new Item((short) ItemType.Firework, 1, 0);
            rocket.Velocity = new Velocity(random.NextDouble() * 0.02 - 0.01, 0.05, random.NextDouble() * 0.02 - 0.01);
        });

        var ticks = random.Next(20, 30);

        animator.After(ticks, () =>
        {
            rocket.Despawn();

            // Where the rocket climbed to, gaining 0.04 blocks per tick each tick.
            var bang = origin with { Y = origin.Y + 0.05 * ticks + 0.02 * ticks * (ticks + 1) };

            Motion.Sound(world, random.Next(3) == 0 ? Sounds.FireworksLargeBlast : Sounds.FireworksBlast, bang, 4F, 0.95F + (float) random.NextDouble() * 0.1F);
            animator.After(10, () => Motion.Sound(world, Sounds.FireworksTwinkle, bang, 4F));
        });

        return rocket;
    }

    /// <summary>
    /// Bursts experience orbs out of a spot, clients pull them towards the closest player until they are collected.
    /// </summary>
    public void Orbs(IWorld world, Position origin, int count)
    {
        var random = Random.Shared;

        for (var index = 0; index < count; index++)
        {
            var orb = world.Spawn<IExperienceOrb>(origin, orb => orb.Count = (short) random.Next(1, 40));
            orb.Push(new Velocity(random.NextDouble() * 0.4 - 0.2, 0.2 + random.NextDouble() * 0.3, random.NextDouble() * 0.4 - 0.2));

            animator.After(random.Next(40, 80), () =>
            {
                orb.Despawn();

                // Nobody really collects them, so everyone nearby hears the pick up.
                foreach (var pair in world.Players.Where(pair => Motion.Distance(pair.Value.Position, origin) < 16))
                {
                    Motion.Sound(world, Sounds.RandomOrb, pair.Value.Position, 0.3F, 0.6F + (float) random.NextDouble() * 0.9F);
                }
            });
        }
    }

    /// <summary>
    /// Lights a block of TNT, which blows up once its fuse burns out.
    /// </summary>
    public IPrimedTnt Prime(IWorld world, Position position)
    {
        var tnt = world.Spawn<IPrimedTnt>(position);
        Motion.Sound(world, Sounds.CreeperPrimed, position);

        // Clients remove primed TNT on their own after the same 80 ticks.
        animator.After(80, () =>
        {
            tnt.Despawn();
            Explode(world, position, 4);
        });

        return tnt;
    }

    public void Strike(IWorld world, Position position)
    {
        world.Strike(position);

        Motion.Sound(world, Sounds.AmbientWeatherThunder, position, 10000F, 0.8F + (float) Random.Shared.NextDouble() * 0.2F);
        Motion.Sound(world, Sounds.RandomExplode, position, 2F, 0.5F + (float) Random.Shared.NextDouble() * 0.2F);

        Ignite(world, position);
    }

    public void Explode(IWorld world, Position position, double radius)
    {
        WandSubscriber.Blast(world, (int) Math.Floor(position.X), (int) Math.Floor(position.Y), (int) Math.Floor(position.Z), radius);
        Motion.Sound(world, Sounds.RandomExplode, position, 4F, 0.7F + (float) Random.Shared.NextDouble() * 0.2F);
    }

    private static void Ignite(IWorld world, Position position)
    {
        var where = new Position(Math.Floor(position.X), Math.Floor(position.Y), Math.Floor(position.Z));

        if (world[where].Type is 0 && Motion.Solid(world, where with { Y = where.Y - 1 }))
        {
            world.Set(where, Blocks.Fire);
        }
    }
}
