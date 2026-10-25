using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Worlds;
using Amethyst.Example.Creatures.Behaviors;
using Amethyst.Extensions;

namespace Amethyst.Example.Creatures;

/// <summary>
/// Where an entity is summoned and who summoned it.
/// </summary>
/// <param name="Position">Where the entity appears, projectiles fly from the summoner's eyes instead.</param>
/// <param name="Yaw">Where the summoner looks, which the entity faces.</param>
internal sealed record Summoning(IWorld World, Position Position, float Yaw, float Pitch, IPlayer? Summoner);

/// <summary>
/// Every entity of the game by name, each summoned with a little variety.
/// </summary>
internal sealed class Bestiary
{
    /// <summary>
    /// The names of every entity, as typed in commands.
    /// </summary>
    public string[] Names { get; }

    /// <summary>
    /// The names of the mobs, which walk, swim or fly around once summoned.
    /// </summary>
    public string[] Mobs { get; }

    // Spawn eggs are told apart by their durability, which is the mob's network identifier.
    private static readonly Dictionary<short, string> eggs = new()
    {
        [50] = "creeper",
        [51] = "skeleton",
        [52] = "spider",
        [53] = "giant",
        [54] = "zombie",
        [55] = "slime",
        [56] = "ghast",
        [57] = "zombie_pigman",
        [58] = "enderman",
        [59] = "cave_spider",
        [60] = "silverfish",
        [61] = "blaze",
        [62] = "magma_cube",
        [63] = "ender_dragon",
        [64] = "wither",
        [65] = "bat",
        [66] = "witch",
        [67] = "endermite",
        [68] = "guardian",
        [90] = "pig",
        [91] = "sheep",
        [92] = "cow",
        [93] = "chicken",
        [94] = "squid",
        [95] = "wolf",
        [96] = "mooshroom",
        [97] = "snow_golem",
        [98] = "ocelot",
        [99] = "iron_golem",
        [100] = "horse",
        [101] = "rabbit",
        [120] = "villager"
    };

    private static readonly Block[] carried = [Blocks.GrassBlock, Blocks.Explosive, Blocks.Cactus, Blocks.Pumpkin, Blocks.Melon, Blocks.Poppy, Blocks.Sand, Blocks.MossStone];

    private static readonly Block[] cargo = [Blocks.Pumpkin, Blocks.JackoLantern, Blocks.Melon, Blocks.HayBale, Blocks.Glowstone, Blocks.Diamond, Blocks.Bookshelf, Blocks.Explosive];

    private static readonly Block[] falling = [Blocks.Sand, Blocks.Gravel, Blocks.Anvil, Blocks.DragonEgg, new(12, 1)];

    private static readonly ItemType[] loot =
    [
        ItemType.Diamond,
        ItemType.Emerald,
        ItemType.GoldenApple,
        ItemType.Cake,
        ItemType.DiamondSword,
        ItemType.NetherStar,
        ItemType.Compass,
        ItemType.Watch,
        ItemType.EnderPearl,
        ItemType.Bone,
        ItemType.Pork,
        ItemType.Rabbit
    ];

    // Splash potions of regeneration, swiftness, fire resistance, poison, healing, night vision,
    // weakness, strength, slowness, harming, water breathing and invisibility.
    private static readonly short[] potions = [16385, 16386, 16387, 16388, 16389, 16390, 16392, 16393, 16394, 16396, 16397, 16398];

    private static readonly (ItemType Helmet, ItemType Chestplate, ItemType Leggings, ItemType Boots)[] armors =
    [
        (ItemType.LeatherHelmet, ItemType.LeatherChestplate, ItemType.LeatherLeggings, ItemType.LeatherBoots),
        (ItemType.ChainmailHelmet, ItemType.ChainmailChestplate, ItemType.ChainmailLeggings, ItemType.ChainmailBoots),
        (ItemType.IronHelmet, ItemType.IronChestplate, ItemType.IronLeggings, ItemType.IronBoots),
        (ItemType.GoldHelmet, ItemType.GoldChestplate, ItemType.GoldLeggings, ItemType.GoldBoots),
        (ItemType.DiamondHelmet, ItemType.DiamondChestplate, ItemType.DiamondLeggings, ItemType.DiamondBoots)
    ];

    private readonly Dictionary<string, Func<Summoning, ISpawnable?>> entries;
    private readonly Realm realm;
    private readonly Animator animator;
    private readonly Ballistics ballistics;

    public Bestiary(Realm realm, Animator animator, Ballistics ballistics)
    {
        this.realm = realm;
        this.animator = animator;
        this.ballistics = ballistics;

        entries = new Dictionary<string, Func<Summoning, ISpawnable?>>
        {
            // Mobs.
            ["bat"] = Mob<IBat>(),
            ["blaze"] = Mob<IBlaze>(blaze => blaze.IsCharging = Chance(2)),
            ["cave_spider"] = Mob<ICaveSpider>(spider => spider.IsClimbing = Chance(5)),
            ["chicken"] = Mob<IChicken>(chicken => chicken.IsChild = Chance(4)),
            ["cow"] = Mob<ICow>(cow => cow.IsChild = Chance(4)),
            ["creeper"] = Mob<ICreeper>(Charge),
            ["ender_dragon"] = Mob<IEnderDragon>(dragon => dragon.CustomName = "Ender Dragon"),
            ["enderman"] = Mob<IEnderman>(Burden),
            ["endermite"] = Mob<IEndermite>(),
            ["ghast"] = Mob<IGhast>(ghast => ghast.IsAttacking = Chance(3)),
            ["giant"] = Mob<IGiant>(giant => giant.Held = Gear(ItemType.DiamondSword)),
            ["guardian"] = Mob<IGuardian>(Watch),
            ["horse"] = Mob<IHorse>(Saddle),
            ["iron_golem"] = Mob<IIronGolem>(golem => golem.IsPlayerCreated = Chance(2)),
            ["magma_cube"] = Mob<IMagmaCube>(Grow),
            ["mooshroom"] = Mob<IMooshroom>(cow => cow.IsChild = Chance(4)),
            ["ocelot"] = Mob<IOcelot>(Purr),
            ["pig"] = Mob<IPig>(Ride),
            ["rabbit"] = Mob<IRabbit>(Hop),
            ["sheep"] = Mob<ISheep>(Fleece),
            ["silverfish"] = Mob<ISilverfish>(),
            ["skeleton"] = Mob<ISkeleton>(Arm),
            ["slime"] = Mob<ISlime>(Grow),
            ["snow_golem"] = Mob<ISnowGolem>(),
            ["spider"] = Mob<ISpider>(spider => spider.IsClimbing = Chance(5)),
            ["squid"] = Mob<ISquid>(),
            ["villager"] = Mob<IVillager>(Employ),
            ["witch"] = Mob<IWitch>(Brew),
            ["wither"] = Mob<IWither>(wither => wither.CustomName = "Wither"),
            ["wolf"] = Mob<IWolf>(Collar),
            ["zombie"] = Mob<IZombie>(Rot),
            ["zombie_pigman"] = Mob<IZombiePigman>(Rot),

            // Objects.
            ["armor_stand"] = Thing<IArmorStand>(Pose),
            ["arrow"] = summoning => ballistics.Shoot(summoning.World, From(summoning), Aim(summoning), summoning.Summoner),
            ["boat"] = Thing<IBoat>(),
            ["chest_minecart"] = Thing<IChestMinecart>(),
            ["command_block_minecart"] = Thing<ICommandBlockMinecart>(Program),
            ["dropped_item"] = Thing<IDroppedItem>(Toss),
            ["egg"] = summoning => ballistics.Throw<IEgg>(summoning.World, From(summoning), Aim(summoning), 1.5, landed: landing => Hatch(summoning, landing)),
            ["ender_crystal"] = Thing<IEnderCrystal>(),
            ["ender_pearl"] = summoning => ballistics.Throw<IEnderPearl>(summoning.World, From(summoning), Aim(summoning), 1.5, landed: landing => Warp(summoning, landing)),
            ["experience_bottle"] = summoning => ballistics.Throw<IExperienceBottle>(summoning.World, From(summoning), Aim(summoning), 0.7, landed: landing => Shatter(summoning, landing)),
            ["experience_orb"] = summoning => Burst(summoning),
            ["eye_of_ender"] = summoning => Rise(summoning),
            ["falling_block"] = summoning => ballistics.Drop(summoning.World, summoning.Position with { Y = summoning.Position.Y + 12 }, Pick(falling)),
            ["firework"] = summoning => ballistics.Firework(summoning.World, summoning.Position),
            ["fishing_hook"] = Thing<IFishingHook>((hook, summoning) => hook.Owner = summoning.Summoner),
            ["furnace_minecart"] = Thing<IFurnaceMinecart>(minecart => minecart.IsPowered = true),
            ["hopper_minecart"] = Thing<IHopperMinecart>(),
            ["item_frame"] = Thing<IItemFrame>(Frame),
            ["large_fireball"] = summoning => Fire<ILargeFireball>(summoning, 3.5),
            ["leash_knot"] = summoning => Tie(summoning),
            ["lightning"] = summoning => Smite(summoning),
            ["painting"] = Thing<IPainting>(Hang),
            ["primed_tnt"] = summoning => ballistics.Prime(summoning.World, summoning.Position),
            ["rideable_minecart"] = Thing<IRideableMinecart>(minecart => minecart.DisplayedBlock = Chance(2) ? Pick(cargo) : null),
            ["small_fireball"] = summoning => Fire<ISmallFireball>(summoning, 0),
            ["snowball"] = summoning => ballistics.Throw<ISnowball>(summoning.World, From(summoning), Aim(summoning), 1.5),
            ["spawner_minecart"] = Thing<ISpawnerMinecart>(),
            ["thrown_potion"] = summoning => Splash(summoning),
            ["tnt_minecart"] = Thing<ITntMinecart>(),
            ["wither_skull"] = summoning => Fire<IWitherSkull>(summoning, 2.5, skull => skull.IsCharged = Chance(2))
        };

        Names = entries.Keys.Order().ToArray();
        Mobs = eggs.Values.Order().ToArray();
    }

    public static string Humanize(string name)
    {
        return string.Join(' ', name.Split('_').Select(word => char.ToUpperInvariant(word[0]) + word[1..]));
    }

    /// <returns>The name of the mob hatched from a spawn egg, or <see langword="null"/> if no mob has that egg.</returns>
    public static string? Egg(short durability)
    {
        return eggs.GetValueOrDefault(durability);
    }

    /// <summary>
    /// Summons an entity and shows it to the players in the world.
    /// </summary>
    /// <param name="name">The entity's name, one of <see cref="Names"/>.</param>
    /// <param name="summoning">Where the entity is summoned and who summoned it.</param>
    /// <param name="roam">Whether mobs walk, swim or fly around on their own.</param>
    /// <param name="label">A name always shown above the entity.</param>
    /// <returns>The entity, or <see langword="null"/> for lightning and orbs which are gone in a moment.</returns>
    public ISpawnable? Summon(string name, Summoning summoning, bool roam = true, string? label = null)
    {
        if (!entries.TryGetValue(name, out var summon) || summon(summoning) is not { } entity)
        {
            return null;
        }

        if (label is not null)
        {
            entity.CustomName = label;
            entity.IsCustomNameVisible = true;
            entity.Refresh();
        }
        else if (entity is IMob && entity.CustomName is null && Chance(40))
        {
            // Every now and then a mob is named Dinnerbone, which clients show upside down.
            entity.CustomName = "Dinnerbone";
            entity.Refresh();
        }

        if (roam && entity is IMob mob)
        {
            Roam(mob, summoning.Position);
        }

        return entity;
    }

    public void Roam(IMob mob, Position home)
    {
        switch (mob)
        {
            case IEnderDragon or IWither or IGhast:
                animator.Add(new Hover(mob, home with { Y = home.Y + 12 }, 16, 0.25));
                break;

            case IBat or IBlaze or ISquid or IGuardian:
                animator.Add(new Hover(mob, home with { Y = home.Y + 3 }, 6));
                break;

            default:
                animator.Add(new Wander(mob, realm, home, 8, mob is ISlime or IMagmaCube or ISilverfish or IEndermite ? 0.08 : 0.12));
                break;
        }
    }

    /// <returns>Whether the mob flies or swims rather than walking.</returns>
    public static bool Flies(IMob mob)
    {
        return mob is IEnderDragon or IWither or IGhast or IBat or IBlaze or ISquid or IGuardian;
    }

    public static bool Chance(int odds)
    {
        return Random.Shared.Next(odds) == 0;
    }

    public static T Pick<T>(T[] values)
    {
        return values[Random.Shared.Next(values.Length)];
    }

    /// <returns>The way a hanging entity faces when looked at from the yaw, in degrees.</returns>
    public static Direction Facing(float yaw)
    {
        // Directions go around clockwise like the yaw, starting from south.
        return (Direction) ((int) Math.Floor((yaw + 180) / 90 + 0.5) & 3);
    }

    private static Func<Summoning, ISpawnable?> Mob<T>(Action<T>? dress = null) where T : IMob
    {
        return Mob<T>((mob, _) => dress?.Invoke(mob));
    }

    private static Func<Summoning, ISpawnable?> Mob<T>(Action<T, Summoning> dress) where T : IMob
    {
        return summoning => summoning.World.Spawn<T>(summoning.Position, mob =>
        {
            mob.Yaw = summoning.Yaw + 180;
            dress(mob, summoning);
        });
    }

    private static Func<Summoning, ISpawnable?> Thing<T>(Action<T>? dress = null) where T : ISpawnable
    {
        return Thing<T>((entity, _) => dress?.Invoke(entity));
    }

    private static Func<Summoning, ISpawnable?> Thing<T>(Action<T, Summoning> dress) where T : ISpawnable
    {
        return summoning => summoning.World.Spawn<T>(summoning.Position, entity =>
        {
            entity.Yaw = summoning.Yaw;
            dress(entity, summoning);
        });
    }

    private ISpawnable Fire<T>(Summoning summoning, double power, Action<T>? configure = null) where T : IFireball
    {
        // Clients only fly fireballs with a shooter, so primed TNT stands in without a summoner.
        if (summoning.Summoner is not { } summoner)
        {
            return ballistics.Prime(summoning.World, summoning.Position);
        }

        Motion.Effect(summoning.World, power > 0 ? Effects.GhastFireball : Effects.BlazeFireball, summoner.Position);
        return ballistics.Fire(summoning.World, From(summoning), Aim(summoning), summoner, power, configure);
    }

    // Projectiles leave from the summoner's eyes, a little ahead so they do not hit them.
    private static Position From(Summoning summoning)
    {
        return summoning.Summoner is { } summoner
            ? Motion.Along(Motion.Eye(summoner), Aim(summoning), 1.5)
            : summoning.Position with { Y = summoning.Position.Y + 1 };
    }

    private static Velocity Aim(Summoning summoning)
    {
        return Motion.Direction(summoning.Yaw, summoning.Pitch);
    }

    private static Item Gear(ItemType type, short durability = 0)
    {
        return new Item((short) type, 1, durability);
    }

    private static void Dress(ILiving living, bool helmet = true)
    {
        var (head, chest, legs, feet) = Pick(armors);

        living.Helmet = helmet && Chance(2) ? Gear(head) : default;
        living.Chestplate = Chance(2) ? Gear(chest) : default;
        living.Leggings = Chance(3) ? Gear(legs) : default;
        living.Boots = Chance(3) ? Gear(feet) : default;
    }

    private static void Charge(ICreeper creeper)
    {
        creeper.IsPowered = Chance(4);
        creeper.IsFusing = Chance(8);
    }

    private static void Burden(IEnderman enderman)
    {
        enderman.Carried = Pick(carried);
        enderman.IsScreaming = Chance(4);
    }

    // Some guardians glare at whoever summoned them with their beam.
    private static void Watch(IGuardian guardian, Summoning summoning)
    {
        if (Chance(4))
        {
            guardian.IsElder = true;
            guardian.Health = 80;
        }

        guardian.Target = Chance(2) ? summoning.Summoner : null;
    }

    private static void Saddle(IHorse horse, Summoning summoning)
    {
        var roll = Random.Shared.Next(12);

        horse.Type = roll switch
        {
            0 => HorseType.Zombie,
            1 => HorseType.Skeleton,
            < 4 => HorseType.Donkey,
            < 6 => HorseType.Mule,
            _ => HorseType.Horse
        };

        horse.Color = Pick(Enum.GetValues<HorseColor>());
        horse.Markings = Pick(Enum.GetValues<HorseMarkings>());
        horse.IsChild = Chance(5);

        if (horse.IsChild)
        {
            return;
        }

        horse.IsTamed = Chance(2);
        horse.Owner = horse.IsTamed ? summoning.Summoner?.Guid : null;
        horse.HasSaddle = Chance(2);

        // Only horses wear armor, and only donkeys and mules carry chests.
        if (horse.Type is HorseType.Horse)
        {
            horse.Armor = Pick(Enum.GetValues<HorseArmor>());
        }
        else if (horse.Type is HorseType.Donkey or HorseType.Mule)
        {
            horse.HasChest = Chance(2);
        }
    }

    private static void Ride(IPig pig)
    {
        pig.HasSaddle = Chance(3);
        pig.IsChild = !pig.HasSaddle && Chance(4);
    }

    private static void Grow(ISlime slime)
    {
        slime.Size = Pick<byte>([1, 2, 4]);
        slime.Health = slime.Size * slime.Size;
    }

    private static void Purr(IOcelot ocelot, Summoning summoning)
    {
        ocelot.Type = Pick(Enum.GetValues<OcelotType>());

        if (ocelot.Type is OcelotType.Wild)
        {
            return;
        }

        ocelot.IsTamed = true;
        ocelot.Owner = summoning.Summoner?.Guid;
        ocelot.IsSitting = Chance(3);
    }

    private static void Hop(IRabbit rabbit)
    {
        rabbit.IsChild = Chance(5);

        // The killer bunny is rare, and so is the one clients show with a memorial texture.
        if (Chance(15))
        {
            rabbit.Type = RabbitType.Killer;
            rabbit.CustomName = "The Killer Bunny";
            return;
        }

        rabbit.Type = (RabbitType) Random.Shared.Next(6);

        if (Chance(15))
        {
            rabbit.CustomName = "Toast";
        }
    }

    private static void Fleece(ISheep sheep)
    {
        // Like vanilla, most sheep are white and pink ones are rare.
        var roll = Random.Shared.Next(1000);

        sheep.Color = roll switch
        {
            < 50 => DyeColor.Black,
            < 100 => DyeColor.Gray,
            < 150 => DyeColor.LightGray,
            < 180 => DyeColor.Brown,
            < 182 => DyeColor.Pink,
            < 230 => Pick(Enum.GetValues<DyeColor>()),
            _ => DyeColor.White
        };

        sheep.IsChild = Chance(5);
        sheep.IsSheared = !sheep.IsChild && Chance(6);

        // Clients cycle a sheep named jeb_ through every color.
        if (Chance(8))
        {
            sheep.CustomName = "jeb_";
        }
    }

    private static void Arm(ISkeleton skeleton)
    {
        skeleton.Type = Chance(3) ? SkeletonType.Wither : SkeletonType.Normal;
        skeleton.Held = Gear(skeleton.Type is SkeletonType.Wither ? ItemType.StoneSword : ItemType.Bow);

        Dress(skeleton);
    }

    private static void Employ(IVillager villager)
    {
        villager.Profession = Pick(Enum.GetValues<Profession>());
        villager.IsChild = Chance(5);
    }

    private static void Brew(IWitch witch)
    {
        witch.IsAggressive = Chance(3);
        witch.Held = witch.IsAggressive ? Gear(ItemType.Potion, Pick(potions)) : default;
    }

    private static void Collar(IWolf wolf, Summoning summoning)
    {
        wolf.IsChild = Chance(5);

        if (!Chance(2))
        {
            wolf.IsAngry = Chance(3);
            return;
        }

        wolf.IsTamed = true;
        wolf.Owner = summoning.Summoner?.Guid;
        wolf.CollarColor = Pick(Enum.GetValues<DyeColor>());
        wolf.IsSitting = Chance(4);
    }

    private static void Rot(IZombie zombie)
    {
        zombie.IsChild = Chance(5);

        if (zombie is IZombiePigman)
        {
            zombie.Held = Gear(ItemType.GoldSword);
            return;
        }

        zombie.IsVillager = Chance(4);
        zombie.IsConverting = zombie.IsVillager && Chance(3);
        zombie.Held = Chance(4) ? Gear(Pick([ItemType.IronSpade, ItemType.IronSword, ItemType.Torch])) : default;

        Dress(zombie);
    }

    private static void Pose(IArmorStand stand, Summoning summoning)
    {
        stand.Yaw = summoning.Yaw + 180;
        stand.HasArms = true;
        stand.HasBasePlate = !Chance(3);
        stand.IsSmall = Chance(4);
        stand.Held = Gear(Pick([ItemType.DiamondSword, ItemType.Bow, ItemType.Torch, ItemType.FishingRod, ItemType.Cake]));

        stand.Head = new EulerAngle(Random.Shared.Next(-20, 21), Random.Shared.Next(-30, 31), 0);
        stand.RightArm = new EulerAngle(Random.Shared.Next(-120, 0), 0, Random.Shared.Next(0, 30));
        stand.LeftArm = new EulerAngle(Random.Shared.Next(-120, 0), 0, Random.Shared.Next(-30, 0));

        Dress(stand);
    }

    private static void Program(ICommandBlockMinecart minecart)
    {
        minecart.Command = "/say Hello from Amethyst!";
        minecart.LastOutput = Message.Simple("Hello from Amethyst!");
    }

    private static void Toss(IDroppedItem item)
    {
        item.Item = new Item((short) Pick(loot), (byte) Random.Shared.Next(1, 17), 0);
        item.Velocity = new Velocity(Random.Shared.NextDouble() * 0.2 - 0.1, 0.25, Random.Shared.NextDouble() * 0.2 - 0.1);
    }

    private static void Frame(IItemFrame frame, Summoning summoning)
    {
        frame.Item = Gear(Pick(loot));
        frame.Rotation = (byte) Random.Shared.Next(8);
        frame.Direction = Facing(summoning.Yaw);
    }

    private static void Hang(IPainting painting, Summoning summoning)
    {
        painting.Motive = Pick(Enum.GetValues<Motive>());
        painting.Direction = Facing(summoning.Yaw);
    }

    // One in eight eggs hatches a chick, like vanilla.
    private void Hatch(Summoning summoning, Position landing)
    {
        if (!Chance(8))
        {
            return;
        }

        var chick = summoning.World.Spawn<IChicken>(landing, chick => chick.IsChild = true);
        Roam(chick, landing);
    }

    private static void Warp(Summoning summoning, Position landing)
    {
        if (summoning.Summoner is not { } summoner)
        {
            return;
        }

        summoner.Teleport(landing);
        Motion.Sound(summoning.World, Sounds.MobEndermenPortal, landing);
    }

    private void Shatter(Summoning summoning, Position landing)
    {
        // The bottle's own splash, the metadata picks the particle color.
        Motion.Effect(summoning.World, Effects.SplashPotion, landing);
        ballistics.Orbs(summoning.World, landing, Random.Shared.Next(3, 8));
    }

    private ISpawnable? Burst(Summoning summoning)
    {
        ballistics.Orbs(summoning.World, summoning.Position with { Y = summoning.Position.Y + 0.5 }, 12);
        Motion.Sound(summoning.World, Sounds.RandomLevelUp, summoning.Position);

        return null;
    }

    // Eyes of ender float up until they shatter, since nothing steers them towards a stronghold.
    private IEyeOfEnder Rise(Summoning summoning)
    {
        var eye = summoning.World.Spawn<IEyeOfEnder>(summoning.Position with { Y = summoning.Position.Y + 1 }, eye => eye.Velocity = new Velocity(0, 0.1, 0));

        animator.After(60, () =>
        {
            eye.Despawn();
            Motion.Effect(summoning.World, Effects.EyeOfEnder, eye.Position with { Y = eye.Position.Y + 6 });
        });

        return eye;
    }

    // Leash knots hang on fences, so one is put up first.
    private ILeashKnot Tie(Summoning summoning)
    {
        var post = new Position(Math.Floor(summoning.Position.X), Math.Floor(summoning.Position.Y), Math.Floor(summoning.Position.Z));

        summoning.World.Set(post, Blocks.Fence);
        return summoning.World.Spawn<ILeashKnot>(post);
    }

    private ISpawnable? Smite(Summoning summoning)
    {
        ballistics.Strike(summoning.World, summoning.Position);
        return null;
    }

    private IThrownPotion Splash(Summoning summoning)
    {
        IThrownPotion? potion = null;

        potion = ballistics.Throw<IThrownPotion>(
            summoning.World,
            From(summoning),
            Aim(summoning),
            0.5,
            current => current.Potion = Pick(potions),
            landing => Motion.Effect(summoning.World, Effects.SplashPotion, landing, potion!.Potion));

        return potion;
    }
}
