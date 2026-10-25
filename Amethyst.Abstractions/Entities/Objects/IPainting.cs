namespace Amethyst.Abstractions.Entities.Objects;

/// <remarks>Clients only read the motive when the entity is spawned.</remarks>
public interface IPainting : IHanging
{
    public Motive Motive { get; set; }
}

public enum Motive
{
    Kebab,
    Aztec,
    Alban,
    Aztec2,
    Bomb,
    Plant,
    Wasteland,
    Pool,
    Courbet,
    Sea,
    Sunset,
    Creebet,
    Wanderer,
    Graham,
    Match,
    Bust,
    Stage,
    Void,
    SkullAndRoses,
    Wither,
    Fighters,
    Pointer,
    Pigscene,
    BurningSkull,
    Skeleton,
    DonkeyKong
}