using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Worlds;
using Amethyst.Example.Creatures.Behaviors;
using Amethyst.Extensions;

namespace Amethyst.Example.Creatures;

/// <summary>
/// The attractions, from a zoo of every mob to a carousel, a dance floor and a painting gallery.
/// </summary>
internal sealed class Showcase(Realm realm, Animator animator, Bestiary bestiary, Ballistics ballistics)
{
    // The plaza's hologram and herd are kept when clearing.
    private readonly HashSet<ISpawnable> permanent = [];

    /// <summary>
    /// Floats a hologram above the spawn and lets a herd graze around it, before anyone joins.
    /// </summary>
    public void Plaza()
    {
        var spawn = realm.Spawn;

        // Players join looking south, right at it.
        var lines = Hologram(
            realm.World,
            spawn with { Y = spawn.Y + 3, Z = spawn.Z + 6 },
            "Amethyst Expedition",
            "§7Every 1.8 entity lives here now",
            "§7Try §d/summon§7, §d/zoo§7, §d/pet§7 or §d/party",
            "§7and the new toys in your hotbar");

        animator.Add(new Shimmer(lines[0], "Amethyst Expedition"));
        permanent.UnionWith(lines);

        (string Name, int Count)[] herd = [("sheep", 6), ("cow", 3), ("chicken", 4), ("pig", 2), ("horse", 2), ("rabbit", 3)];

        foreach (var (name, count) in herd)
        {
            for (var index = 0; index < count; index++)
            {
                var angle = Random.Shared.NextDouble() * Math.Tau;
                var distance = 8 + Random.Shared.NextDouble() * 10;

                var x = (int) Math.Floor(spawn.X + Math.Cos(angle) * distance);
                var z = (int) Math.Floor(spawn.Z + Math.Sin(angle) * distance);

                var summoning = new Summoning(realm.World, realm.Surface(x, z), Random.Shared.Next(360), 0, null);

                if (bestiary.Summon(name, summoning) is { } animal)
                {
                    permanent.Add(animal);
                }
            }
        }
    }

    /// <summary>
    /// Floats lines of text, each held up by an invisible marker armor stand.
    /// </summary>
    public IArmorStand[] Hologram(IWorld world, Position top, params string[] lines)
    {
        return lines
            .Select((line, index) => world.Spawn<IArmorStand>(top with { Y = top.Y - index * 0.3 }, stand =>
            {
                stand.IsInvisible = true;
                stand.IsMarker = true;
                stand.HasGravity = false;
                stand.HasBasePlate = false;
                stand.CustomName = line;
                stand.IsCustomNameVisible = true;
            }))
            .ToArray();
    }

    /// <summary>
    /// Rings the player with every mob, each with its name above its head.
    /// </summary>
    public int Zoo(IPlayer player)
    {
        const double radius = 16;

        var mobs = bestiary.Mobs;

        for (var index = 0; index < mobs.Length; index++)
        {
            var angle = index * Math.Tau / mobs.Length;

            var x = (int) Math.Floor(player.Position.X + Math.Cos(angle) * radius);
            var z = (int) Math.Floor(player.Position.Z + Math.Sin(angle) * radius);

            // Looking from the player at the mob, so the mob turns around to face them.
            var yaw = Motion.Yaw(x - player.Position.X, z - player.Position.Z);
            var summoning = new Summoning(player.World, realm.Surface(x, z), yaw, 0, player);

            bestiary.Summon(mobs[index], summoning, label: Bestiary.Humanize(mobs[index]));
        }

        Motion.Sound(player.World, Sounds.RandomLevelUp, player.Position);
        return mobs.Length;
    }

    /// <summary>
    /// Builds a merry-go-round of horses under a striped canopy in front of the player.
    /// </summary>
    public void Carousel(IPlayer player)
    {
        const int radius = 6;

        var world = player.World;
        var ahead = Motion.Along(player.Position, Motion.Direction(player.Yaw, 0), radius + 4);

        var centerX = (int) Math.Floor(ahead.X);
        var centerZ = (int) Math.Floor(ahead.Z);
        var floor = (int) realm.Surface(centerX, centerZ).Y;

        for (var x = -radius; x <= radius; x++)
        {
            for (var z = -radius; z <= radius; z++)
            {
                if (x * x + z * z > radius * radius + 2)
                {
                    continue;
                }

                // Eight slices of alternating colors, like a circus tent.
                var slice = (int) Math.Floor((Math.Atan2(z, x) + Math.PI) / (Math.PI / 4)) % 2;

                Place(world, new Position(centerX + x, floor - 1, centerZ + z), new Block(Blocks.StainedClay.Type, slice is 0 ? 0 : 2));
                Place(world, new Position(centerX + x, floor + 6, centerZ + z), new Block(Blocks.Wool.Type, slice is 0 ? 0 : 14));

                for (var y = floor; y < floor + 6; y++)
                {
                    Place(world, new Position(centerX + x, y, centerZ + z), x is 0 && z is 0 ? y < floor + 5 ? Blocks.Fence : Blocks.Glowstone : Blocks.Air);
                }
            }
        }

        var horses = new IHorse[8];
        var center = new Position(centerX + 0.5, floor, centerZ + 0.5);

        for (var index = 0; index < horses.Length; index++)
        {
            horses[index] = world.Spawn<IHorse>(center with { X = center.X + 4 }, horse =>
            {
                horse.IsTamed = true;
                horse.HasSaddle = true;
                horse.Color = Bestiary.Pick(Enum.GetValues<HorseColor>());
                horse.Markings = Bestiary.Pick(Enum.GetValues<HorseMarkings>());
                horse.Armor = Bestiary.Pick([HorseArmor.Iron, HorseArmor.Gold, HorseArmor.Diamond]);
                horse.HasAi = false;
            });
        }

        var title = Hologram(world, center with { Y = floor + 8.5 }, "Carousel");
        animator.Add(new Shimmer(title[0], "Carousel"));

        animator.Add(new Carousel(center, horses, 4));
    }

    /// <summary>
    /// Lays a glowing dance floor in front of the player with a troupe of dancing armor stands.
    /// </summary>
    public void Dance(IPlayer player)
    {
        // Each dancer wears a different mob head, the durability picks which.
        (short Head, ItemType Chestplate, ItemType Leggings, ItemType Boots, ItemType Held)[] troupe =
        [
            (0, ItemType.ChainmailChestplate, ItemType.ChainmailLeggings, ItemType.ChainmailBoots, ItemType.Bone),
            (2, ItemType.LeatherChestplate, ItemType.LeatherLeggings, ItemType.LeatherBoots, ItemType.RottenFlesh),
            (4, ItemType.GoldChestplate, ItemType.GoldLeggings, ItemType.GoldBoots, ItemType.Sulphur),
            (3, ItemType.DiamondChestplate, ItemType.DiamondLeggings, ItemType.DiamondBoots, ItemType.DiamondSword),
            (1, ItemType.IronChestplate, ItemType.IronLeggings, ItemType.IronBoots, ItemType.NetherStar)
        ];

        var world = player.World;
        var forward = Motion.Direction(player.Yaw, 0);

        // Perpendicular to where the player looks.
        var across = new Velocity(-forward.Z, 0, forward.X);
        var stage = Motion.Along(player.Position, forward, 7);
        var floor = (int) realm.Surface((int) Math.Floor(stage.X), (int) Math.Floor(stage.Z)).Y;

        for (var a = -6; a <= 6; a++)
        {
            for (var b = -2; b <= 2; b++)
            {
                var tile = Motion.Along(Motion.Along(stage, across, a), forward, b);
                var x = (int) Math.Floor(tile.X);
                var z = (int) Math.Floor(tile.Z);

                // A checkerboard of light and colored clay.
                Place(world, new Position(x, floor - 1, z), (x + z) % 2 is 0 ? Blocks.Glowstone : new Block(Blocks.StainedClay.Type, Random.Shared.Next(16)));

                for (var y = floor; y < floor + 3; y++)
                {
                    Place(world, new Position(x, y, z), Blocks.Air);
                }
            }
        }

        var dancers = new IArmorStand[troupe.Length];

        for (var index = 0; index < dancers.Length; index++)
        {
            var (head, chestplate, leggings, boots, held) = troupe[index];
            var spot = Motion.Along(stage, across, (index - dancers.Length / 2) * 2.5) with { Y = floor };

            dancers[index] = world.Spawn<IArmorStand>(spot, dancer =>
            {
                dancer.Yaw = player.Yaw + 180;
                dancer.HasArms = true;
                dancer.HasBasePlate = false;
                dancer.Helmet = new Item((short) ItemType.SkullItem, 1, head);
                dancer.Chestplate = new Item((short) chestplate, 1, 0);
                dancer.Leggings = new Item((short) leggings, 1, 0);
                dancer.Boots = new Item((short) boots, 1, 0);
                dancer.Held = new Item((short) held, 1, 0);
            });
        }

        var title = Hologram(world, stage with { Y = floor + 3.5 }, "Dance Floor");
        animator.Add(new Shimmer(title[0], "Dance Floor"));

        animator.Add(new Dance(stage, dancers));
    }

    /// <summary>
    /// Builds a quartz wall ahead of the player with every painting and a row of item frames.
    /// </summary>
    public void Gallery(IPlayer player)
    {
        var world = player.World;

        // Galleries line up with the block grid, facing the player.
        var yaw = (float) (Math.Floor(player.Yaw / 90 + 0.5) * 90);
        var forward = Motion.Direction(yaw, 0);
        var step = (X: (int) Math.Round(forward.X), Z: (int) Math.Round(forward.Z));
        var across = (X: -step.Z, Z: step.X);

        var wall = (X: (int) Math.Floor(player.Position.X) + step.X * 10, Z: (int) Math.Floor(player.Position.Z) + step.Z * 10);
        var floor = (int) realm.Surface(wall.X - step.X, wall.Z - step.Z).Y;

        var motives = Enum.GetValues<Motive>();
        var facing = Bestiary.Facing(yaw);

        for (var offset = -33; offset <= 33; offset++)
        {
            var x = wall.X + across.X * offset;
            var z = wall.Z + across.Z * offset;

            for (var y = floor - 2; y <= floor + 12; y++)
            {
                Place(world, new Position(x, y, z), y == floor + 12 && offset % 4 is 0 ? Blocks.Glowstone : Blocks.Quartz);

                // Clear the two blocks in front of the wall, so nothing hides the paintings.
                if (y >= floor)
                {
                    Place(world, new Position(x - step.X, y, z - step.Z), Blocks.Air);
                    Place(world, new Position(x - step.X * 2, y, z - step.Z * 2), Blocks.Air);
                }
            }
        }

        // The small paintings hang low, the large ones up high, and item frames below them all.
        for (var index = 0; index < motives.Length; index++)
        {
            var column = index % 13;
            var offset = -30 + column * 5;
            var height = index < 13 ? 4 : 9;

            var hang = new Position(wall.X + across.X * offset - step.X, floor + height, wall.Z + across.Z * offset - step.Z);

            world.Spawn<IPainting>(hang, painting =>
            {
                painting.Motive = motives[index];
                painting.Direction = facing;
            });

            if (index < 13)
            {
                world.Spawn<IItemFrame>(hang with { Y = floor + 1 }, frame =>
                {
                    frame.Item = new Item((short) Bestiary.Pick([ItemType.Diamond, ItemType.GoldenApple, ItemType.Compass, ItemType.Emerald, ItemType.Cake, ItemType.Bow]), 1, 0);
                    frame.Rotation = (byte) Random.Shared.Next(8);
                    frame.Direction = facing;
                });
            }
        }

        var top = new Position(wall.X - step.X + 0.5, floor + 14, wall.Z - step.Z + 0.5);
        var title = Hologram(world, top, "Amethyst Gallery", "§7Every painting of 1.8");

        animator.Add(new Shimmer(title[0], "Amethyst Gallery"));
    }

    /// <summary>
    /// Rains rainbow sheep and a cake, sets off fireworks and bursts experience around the player.
    /// </summary>
    public void Party(IPlayer player)
    {
        var world = player.World;
        var center = player.Position;

        for (var index = 0; index < 16; index++)
        {
            var drop = new Position(
                center.X + Random.Shared.NextDouble() * 16 - 8,
                center.Y + 20 + Random.Shared.NextDouble() * 10,
                center.Z + Random.Shared.NextDouble() * 16 - 8);

            var sheep = world.Spawn<ISheep>(drop, sheep =>
            {
                sheep.Color = Bestiary.Pick(Enum.GetValues<DyeColor>());
                sheep.Yaw = Random.Shared.Next(360);

                // Clients cycle a sheep named jeb_ through every color.
                sheep.CustomName = Bestiary.Chance(3) ? "jeb_" : null;
            });

            // Wandering also makes them fall.
            animator.Add(new Wander(sheep, realm, center, 10));
        }

        for (var index = 0; index < 10; index++)
        {
            var launch = new Position(center.X + Random.Shared.NextDouble() * 12 - 6, center.Y, center.Z + Random.Shared.NextDouble() * 12 - 6);
            animator.After(1 + index * 6, () => ballistics.Firework(world, launch));
        }

        ballistics.Drop(world, Motion.Along(center, Motion.Direction(player.Yaw, 0), 3) with { Y = center.Y + 15 }, Blocks.Cake);
        ballistics.Orbs(world, center with { Y = center.Y + 1 }, 24);

        Motion.Sound(world, Sounds.RandomLevelUp, center);
    }

    /// <summary>
    /// Counts the entities in the world by kind.
    /// </summary>
    public (string Kind, int Count)[] Census(IWorld world)
    {
        return world.Entities.Values
            .GroupBy(entity => Spaced(entity.GetType().Name))
            .Select(group => (group.Key, group.Count()))
            .OrderByDescending(pair => pair.Item2)
            .ToArray();
    }

    /// <summary>
    /// Despawns the entities around a position, except pets and the plaza.
    /// </summary>
    public int Clear(IWorld world, Position center, double radius, Func<ISpawnable, bool> keep)
    {
        var cleared = 0;

        foreach (var entity in world.Entities.Values)
        {
            if (permanent.Contains(entity) || keep(entity) || Motion.Distance(entity.Position, center) > radius)
            {
                continue;
            }

            entity.Despawn();
            cleared++;
        }

        return cleared;
    }

    // Only sends blocks that change, building over open ground is mostly air already.
    private static void Place(IWorld world, Position position, Block block)
    {
        var current = world[position];

        if (current.Type != block.Type || current.Metadata != block.Metadata)
        {
            world.Set(position, block);
        }
    }

    // The entities' own classes are named like their interfaces, such as "ZombiePigman".
    private static string Spaced(string name)
    {
        var builder = new System.Text.StringBuilder(name.Length + 4);

        foreach (var character in name)
        {
            if (char.IsUpper(character) && builder.Length > 0)
            {
                builder.Append(' ');
            }

            builder.Append(character);
        }

        return builder.ToString();
    }
}
