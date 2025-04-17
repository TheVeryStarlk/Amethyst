namespace Amethyst.Abstractions.Packets.Play;

public sealed class StatisticsPacket(IDictionary<string, int> values) : IOutgoingPacket
{
    public int Identifier => 55;

    public IDictionary<string, int> Values => values;
}

public static class Statistic
{
    public static string Leaving => "stat.leaveGame";

    public static string Playing => "stat.playOneMinute";

    public static string Walking => "stat.walkOneCm";

    public static string Swimming => "stat.swimOneCm";

    public static string Falling => "stat.fallOneCm";

    public static string Climbing => "stat.climbOneCm";

    public static string Flying => "stat.flyOneCm";

    public static string Diving => "stat.diveOneCm";

    public static string Minecart => "stat.minecartOneCm";

    public static string Boating => "stat.boatOneCm";

    public static string Pig => "stat.pigOneCm";

    public static string Horse => "stat.horseOneCm";

    public static string Jumps => "stat.jump";

    public static string Drops => "stat.drop";

    public static string TookDamage => "stat.damageDealt";

    public static string DidDamage => "stat.damageTaken";

    public static string Deaths => "stat.deaths";

    public static string MobKills => "stat.mobKills";

    public static string AnimalsBred => "stat.animalsBred";

    public static string PlayerKills => "stat.playerKills";

    public static string FishCaught => "stat.fishCaught";

    public static string JunkFished => "stat.junkFished";

    public static string TreasureFished => "stat.treasureFished";
}