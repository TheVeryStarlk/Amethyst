namespace Amethyst.Abstractions;

/// <summary>
/// The identifiers of every effect a client knows, to be played with a <see cref="Packets.Play.EffectPacket"/>.
/// </summary>
public static class Effects
{
    public const int Click = 1000;

    public const int ClickFailed = 1001;

    public const int Bow = 1002;

    public const int Door = 1003;

    public const int Fizz = 1004;

    /// <summary>
    /// Plays the record whose item identifier is the data, or stops the record playing with zero.
    /// </summary>
    public const int Record = 1005;

    public const int GhastCharge = 1007;

    public const int GhastFireball = 1008;

    public const int BlazeFireball = 1009;

    public const int ZombieWood = 1010;

    public const int ZombieMetal = 1011;

    public const int ZombieWoodBreak = 1012;

    public const int WitherSpawn = 1013;

    public const int WitherShoot = 1014;

    public const int BatTakeoff = 1015;

    public const int ZombieInfect = 1016;

    public const int ZombieUnfect = 1017;

    public const int EnderDragonEnd = 1018;

    public const int AnvilBreak = 1020;

    public const int AnvilUse = 1021;

    public const int AnvilLand = 1022;

    /// <summary>
    /// Spawns smoke, the data is the direction it goes towards.
    /// </summary>
    public const int Smoke = 2000;

    /// <summary>
    /// Plays the sound and particles of a breaking block, the data is the block's state.
    /// </summary>
    public const int BlockBreak = 2001;

    /// <summary>
    /// Spawns the particles of a splashing potion, the data is the potion's identifier.
    /// </summary>
    public const int SplashPotion = 2002;

    public const int EyeOfEnder = 2003;

    public const int MobSpawn = 2004;

    /// <summary>
    /// Spawns the particles of bone meal, the data is the amount of them.
    /// </summary>
    public const int BoneMeal = 2005;
}