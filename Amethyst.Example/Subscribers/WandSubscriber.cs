using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Eventing.Player;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Worlds;
using Amethyst.Example.Generation;
using Amethyst.Extensions;

namespace Amethyst.Example.Subscribers;

/// <summary>
/// Brings the kit's wands to life.
/// </summary>
internal sealed class WandSubscriber(Realm realm) : ISubscriber
{
    public void Subscribe(IRegistry registry)
    {
        registry.For<IPlayer>(consumer => consumer.On<Use>((player, use) =>
        {
            // Right-clicking the air reports a position of -1, -1, -1.
            var block = use.Position.Y is >= 0 and < 256;

            var x = (int) use.Position.X;
            var y = (int) use.Position.Y;
            var z = (int) use.Position.Z;

            switch ((ItemType) use.Item.Type)
            {
                case ItemType.Stick when block:
                    Grow(player, x, y, z);
                    break;

                case ItemType.BlazeRod when block:
                    Blast(player.World, x, y, z);
                    break;

                case ItemType.Feather when !block:
                    Blink(player);
                    break;

                case ItemType.InkSack when block && use.Item.Durability is 15:
                    Bloom(player.World, x, y, z);
                    break;
            }
        }));
    }

    private void Grow(IPlayer player, int x, int y, int z)
    {
        var biome = realm.Generator.Sample(x, z).Biome;
        var kind = Trees.Pick(biome, Random.Shared);

        Trees.Grow(new WorldCanvas(player.World), kind, x, y + 1, z, Random.Shared);

        var name = kind.Humanize().ToLowerInvariant();
        var article = name[0] is 'a' or 'e' or 'i' or 'o' or 'u' ? "An " : "A ";

        player.Send(Chat.Of(article.Paint(Color.Gray), name.Paint(Color.Green), " tree sprouted!".Paint(Color.Gray)), MessagePosition.HotBar);
    }

    private static void Blast(IWorld world, int x, int y, int z)
    {
        const double radius = 3.5;

        var random = Random.Shared;

        for (var offsetX = -5; offsetX <= 5; offsetX++)
        {
            for (var offsetY = -5; offsetY <= 5; offsetY++)
            {
                for (var offsetZ = -5; offsetZ <= 5; offsetZ++)
                {
                    var current = (X: x + offsetX, Y: y + offsetY, Z: z + offsetZ);

                    if (current.Y is < 1 or > 255)
                    {
                        continue;
                    }

                    // A slightly noisy sphere, so craters do not look machined.
                    var distance = Math.Sqrt(offsetX * offsetX + offsetY * offsetY + offsetZ * offsetZ) + random.NextDouble() * 0.6;
                    var position = new Position(current.X, current.Y, current.Z);

                    if (distance < radius)
                    {
                        if (world[current.X, current.Y, current.Z].Type is not 0)
                        {
                            world.Set(position, Blocks.Air);
                        }
                    }
                    else if (distance < radius + 1 && world[current.X, current.Y, current.Z].Type is not (0 or 7) && random.Next(3) == 0)
                    {
                        // Scorch the walls, and set some of the floor alight.
                        world.Set(position, Flora.Netherrack);

                        if (offsetY < 0 && world[current.X, current.Y + 1, current.Z].Type is 0 && random.Next(3) == 0)
                        {
                            world.Set(position with { Y = current.Y + 1 }, Blocks.Fire);
                        }
                    }
                }
            }
        }
    }

    // Steps along where the player is looking, climbing single blocks, until something is in the way.
    private static void Blink(IPlayer player)
    {
        var yaw = player.Yaw * Math.PI / 180;
        var pitch = player.Pitch * Math.PI / 180;

        var direction = (X: -Math.Sin(yaw) * Math.Cos(pitch), Y: -Math.Sin(pitch), Z: Math.Cos(yaw) * Math.Cos(pitch));
        var destination = player.Position;

        for (var step = 0; step < 48; step++)
        {
            var next = new Position(
                destination.X + direction.X * 0.5,
                destination.Y + direction.Y * 0.5,
                destination.Z + direction.Z * 0.5);

            if (Blocked(player.World, next))
            {
                next = next with { Y = Math.Floor(next.Y) + 1 };

                if (Blocked(player.World, next))
                {
                    break;
                }
            }

            destination = next;
        }

        player.Teleport(destination);
    }

    private static bool Blocked(IWorld world, Position feet)
    {
        return feet.Y is < 1 or > 254 || Solid(world, feet) || Solid(world, feet with { Y = feet.Y + 1 });
    }

    private static void Bloom(IWorld world, int x, int y, int z)
    {
        const int radius = 4;

        var random = Random.Shared;

        for (var offsetX = -radius; offsetX <= radius; offsetX++)
        {
            for (var offsetZ = -radius; offsetZ <= radius; offsetZ++)
            {
                if (offsetX * offsetX + offsetZ * offsetZ > radius * radius)
                {
                    continue;
                }

                // Follow the ground up and down hills.
                for (var level = Math.Min(y + 3, 253); level >= Math.Max(y - 3, 1); level--)
                {
                    if (world[x + offsetX, level, z + offsetZ].Type != Blocks.GrassBlock.Type || world[x + offsetX, level + 1, z + offsetZ].Type is not 0)
                    {
                        continue;
                    }

                    var above = new Position(x + offsetX, level + 1, z + offsetZ);
                    var roll = random.NextDouble();

                    if (roll < 0.08 && world[x + offsetX, level + 2, z + offsetZ].Type is 0)
                    {
                        world.Set(above, Flora.Bush(random));
                        world.Set(above with { Y = level + 2 }, Flora.UpperHalf);
                    }
                    else if (roll < 0.5)
                    {
                        world.Set(above, Flora.Flower(random));
                    }
                    else if (roll < 0.8)
                    {
                        world.Set(above, Flora.TallGrass);
                    }

                    break;
                }
            }
        }
    }

    private static bool Solid(IWorld world, Position position)
    {
        return !Flora.IsPassable(world[(int) Math.Floor(position.X), (int) Math.Floor(position.Y), (int) Math.Floor(position.Z)]);
    }
}
