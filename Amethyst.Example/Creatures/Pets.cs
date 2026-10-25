using System.Collections.Concurrent;
using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Messages;
using Amethyst.Example.Creatures.Behaviors;
using Amethyst.Example.Subscribers;
using Amethyst.Extensions;

namespace Amethyst.Example.Creatures;

/// <summary>
/// A companion for each player, any mob will do.
/// </summary>
internal sealed class Pets(Realm realm, Animator animator, Bestiary bestiary)
{
    private readonly ConcurrentDictionary<IPlayer, ISpawnable> pets = [];

    public bool IsPet(ISpawnable entity)
    {
        return pets.Values.Contains(entity);
    }

    /// <returns>The new pet, or <see langword="null"/> if no mob has the name.</returns>
    public ISpawnable? Adopt(IPlayer player, string kind)
    {
        if (!bestiary.Mobs.Contains(kind))
        {
            return null;
        }

        Dismiss(player);

        // Pets arrive behind their owner.
        var behind = Motion.Along(player.Position, Motion.Direction(player.Yaw, 0), -2);
        var summoning = new Summoning(player.World, behind with { Y = realm.Surface((int) Math.Floor(behind.X), (int) Math.Floor(behind.Z)).Y }, player.Yaw + 180, 0, player);

        if (bestiary.Summon(kind, summoning, false, $"{player.Username}'s {Bestiary.Humanize(kind)}") is not { } pet)
        {
            return null;
        }

        // Pets are tamed by their owner and wear their chat color.
        switch (pet)
        {
            case IWolf wolf:
                wolf.IsTamed = true;
                wolf.IsSitting = false;
                wolf.IsAngry = false;
                wolf.Owner = player.Guid;
                wolf.CollarColor = Dye(ChatSubscriber.Tint(player));
                break;

            case ITameable tameable:
                tameable.IsTamed = true;
                tameable.IsSitting = false;
                tameable.Owner = player.Guid;
                break;

            case IHorse horse:
                horse.IsTamed = true;
                horse.Owner = player.Guid;
                horse.HasSaddle = !horse.IsChild;
                break;
        }

        pet.Update();
        pets[player] = pet;

        animator.Add(new Follow(pet, player, realm, pet is IMob mob && Bestiary.Flies(mob)));
        Motion.Effect(pet.World, Effects.MobSpawn, pet.Position);

        return pet;
    }

    /// <returns>Whether the player had a pet to send home.</returns>
    public bool Dismiss(IPlayer player)
    {
        if (!pets.TryRemove(player, out var pet))
        {
            return false;
        }

        Motion.Effect(pet.World, Effects.MobSpawn, pet.Position);
        pet.Despawn();

        return true;
    }

    private static DyeColor Dye(Color color)
    {
        return color switch
        {
            Color.Aqua => DyeColor.LightBlue,
            Color.Gold => DyeColor.Orange,
            Color.Green => DyeColor.Lime,
            Color.LightPurple => DyeColor.Magenta,
            Color.Yellow => DyeColor.Yellow,
            Color.Blue => DyeColor.Blue,
            _ => DyeColor.Red
        };
    }
}
