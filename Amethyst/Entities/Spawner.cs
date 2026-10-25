using System.Collections.Frozen;
using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Worlds;
using Amethyst.Entities.Mobs;

namespace Amethyst.Entities;

internal static class Spawner
{
    // Keyed by the entity's interface, so abstract ones like ILiving are never created.
    private static readonly FrozenDictionary<Type, Func<IWorld, Spawnable>> spawnables = new Dictionary<Type, Func<IWorld, Spawnable>>
    {
        // Mobs.
        [typeof(IBat)] = world => new Bat(world),
        [typeof(IBlaze)] = world => new Blaze(world),
        [typeof(ICaveSpider)] = world => new CaveSpider(world),
        [typeof(IChicken)] = world => new Chicken(world),
        [typeof(ICow)] = world => new Cow(world),
        [typeof(ICreeper)] = world => new Creeper(world),
        [typeof(IEnderDragon)] = world => new EnderDragon(world),
        [typeof(IEnderman)] = world => new Enderman(world),
        [typeof(IEndermite)] = world => new Endermite(world),
        [typeof(IGhast)] = world => new Ghast(world),
        [typeof(IGiant)] = world => new Giant(world),
        [typeof(IGuardian)] = world => new Guardian(world),
        [typeof(IHorse)] = world => new Horse(world),
        [typeof(IIronGolem)] = world => new IronGolem(world),
        [typeof(IMagmaCube)] = world => new MagmaCube(world),
        [typeof(IMooshroom)] = world => new Mooshroom(world),
        [typeof(IOcelot)] = world => new Ocelot(world),
        [typeof(IPig)] = world => new Pig(world),
        [typeof(IRabbit)] = world => new Rabbit(world),
        [typeof(ISheep)] = world => new Sheep(world),
        [typeof(ISilverfish)] = world => new Silverfish(world),
        [typeof(ISkeleton)] = world => new Skeleton(world),
        [typeof(ISlime)] = world => new Slime(world),
        [typeof(ISnowGolem)] = world => new SnowGolem(world),
        [typeof(ISpider)] = world => new Spider(world),
        [typeof(ISquid)] = world => new Squid(world),
        [typeof(IVillager)] = world => new Villager(world),
        [typeof(IWitch)] = world => new Witch(world),
        [typeof(IWither)] = world => new Wither(world),
        [typeof(IWolf)] = world => new Wolf(world),
        [typeof(IZombie)] = world => new Zombie(world),
        [typeof(IZombiePigman)] = world => new ZombiePigman(world)
    }.ToFrozenDictionary();

    public static Spawnable Create(Type type, IWorld world)
    {
        return spawnables.TryGetValue(type, out var create)
            ? create(world)
            : throw new ArgumentException($"{type.Name} is not an entity that can be created.", nameof(type));
    }
}