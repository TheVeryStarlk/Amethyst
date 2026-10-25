using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Entities;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class SpawnMobSerializer(
    int unique,
    byte type,
    int x,
    int y,
    int z,
    byte yaw,
    byte pitch,
    short velocityX,
    short velocityY,
    short velocityZ,
    Metadata metadata) : ISerializer<SpawnMobPacket, SpawnMobSerializer>
{
    public int Length => Variable.GetByteCount(unique) + sizeof(byte) + sizeof(int) * 3 + sizeof(byte) * 3 + sizeof(short) * 3 + metadata.Length;

    public static SpawnMobSerializer Create(SpawnMobPacket packet)
    {
        var mob = packet.Mob;

        // Mobs extending other mobs come first.
        byte type = mob switch
        {
            ICreeper => 50,
            ISkeleton => 51,
            ICaveSpider => 59,
            ISpider => 52,
            IGiant => 53,
            IZombiePigman => 57,
            IZombie => 54,
            IMagmaCube => 62,
            ISlime => 55,
            IGhast => 56,
            IEnderman => 58,
            ISilverfish => 60,
            IBlaze => 61,
            IEnderDragon => 63,
            IWither => 64,
            IBat => 65,
            IWitch => 66,
            IEndermite => 67,
            IGuardian => 68,
            IPig => 90,
            ISheep => 91,
            IMooshroom => 96,
            ICow => 92,
            IChicken => 93,
            ISquid => 94,
            IWolf => 95,
            ISnowGolem => 97,
            IOcelot => 98,
            IIronGolem => 99,
            IHorse => 100,
            IRabbit => 101,
            IVillager => 120,
            _ => throw new ArgumentOutOfRangeException(nameof(packet), mob, "Unknown mob.")
        };

        // Positions are fixed-point with 5 fractional bits, angles are in 1/256 of a full turn.
        // Angles go through an integer first so negative ones wrap instead of saturating.
        // Velocities are in 1/8000 of a block per tick, capped like clients do.
        return new SpawnMobSerializer(
            mob.Unique,
            type,
            (int) (mob.Position.X * 32D),
            (int) (mob.Position.Y * 32D),
            (int) (mob.Position.Z * 32D),
            (byte) (int) (mob.Yaw / 360F * 256F),
            (byte) (int) (mob.Pitch / 360F * 256F),
            (short) (Math.Clamp(mob.Velocity.X, -3.9D, 3.9D) * 8000D),
            (short) (Math.Clamp(mob.Velocity.Y, -3.9D, 3.9D) * 8000D),
            (short) (Math.Clamp(mob.Velocity.Z, -3.9D, 3.9D) * 8000D),
            Metadata.Create(mob));
    }

    public void Write(Span<byte> span)
    {
        // The head is turned the same way as the body.
        var writer = SpanWriter
            .Create(span)
            .WriteVariableInteger(unique)
            .WriteByte(type)
            .WriteInteger(x)
            .WriteInteger(y)
            .WriteInteger(z)
            .WriteByte(yaw)
            .WriteByte(pitch)
            .WriteByte(yaw)
            .WriteShort(velocityX)
            .WriteShort(velocityY)
            .WriteShort(velocityZ);

        metadata.Write(ref writer);
    }
}