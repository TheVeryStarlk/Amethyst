using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class SpawnObjectSerializer(
    int unique,
    byte type,
    int x,
    int y,
    int z,
    byte pitch,
    byte yaw,
    int data,
    short velocityX,
    short velocityY,
    short velocityZ) : ISerializer<SpawnObjectPacket, SpawnObjectSerializer>
{
    // The velocity is only written if the data is positive.
    public int Length => Variable.GetByteCount(unique) + sizeof(byte) + sizeof(int) * 3 + sizeof(byte) * 2 + sizeof(int) + (data > 0 ? sizeof(short) * 3 : 0);

    public static SpawnObjectSerializer Create(SpawnObjectPacket packet)
    {
        var entity = packet.Entity;

        var (type, data) = entity switch
        {
            IBoat => (1, 0),

            // Items always have data so their velocity is written.
            IDroppedItem => (2, 1),

            // Minecarts share a type, the data tells them apart.
            IRideableMinecart => (10, 0),
            IChestMinecart => (10, 1),
            IFurnaceMinecart => (10, 2),
            ITntMinecart => (10, 3),
            ISpawnerMinecart => (10, 4),
            IHopperMinecart => (10, 5),
            ICommandBlockMinecart => (10, 6),

            IPrimedTnt => (50, 0),
            IEnderCrystal => (51, 0),

            // Arrows without a shooter point to themselves like vanilla does, so their velocity is still written.
            IArrow arrow => (60, arrow.Shooter?.Unique ?? arrow.Unique),

            ISnowball => (61, 0),
            IEgg => (62, 0),
            ILargeFireball fireball => (63, fireball.Shooter?.Unique ?? 0),
            ISmallFireball fireball => (64, fireball.Shooter?.Unique ?? 0),
            IEnderPearl => (65, 0),
            IWitherSkull skull => (66, skull.Shooter?.Unique ?? 0),

            // Unlike in chunks, the block's state here keeps the metadata in the upper bits.
            IFallingBlock falling => (70, falling.Block.Type | (falling.Block.Metadata & 15) << 12),

            IItemFrame frame => (71, (int) frame.Direction),
            IEyeOfEnder => (72, 0),
            IThrownPotion potion => (73, potion.Potion),
            IExperienceBottle => (75, 0),
            IFirework => (76, 0),
            ILeashKnot => (77, 0),
            IArmorStand => (78, 0),
            IFishingHook hook => (90, hook.Owner?.Unique ?? 0),
            _ => throw new ArgumentOutOfRangeException(nameof(packet), entity, "Unknown object.")
        };

        // Positions are fixed-point with 5 fractional bits, angles are in 1/256 of a full turn.
        // Angles go through an integer first so negative ones wrap instead of saturating.
        // Velocities are in 1/8000 of a block per tick, capped like clients do.
        return new SpawnObjectSerializer(
            entity.Unique,
            (byte) type,
            (int) (entity.Position.X * 32D),
            (int) (entity.Position.Y * 32D),
            (int) (entity.Position.Z * 32D),
            (byte) (int) (entity.Pitch / 360F * 256F),
            (byte) (int) (entity.Yaw / 360F * 256F),
            data,
            (short) (Math.Clamp(entity.Velocity.X, -3.9D, 3.9D) * 8000D),
            (short) (Math.Clamp(entity.Velocity.Y, -3.9D, 3.9D) * 8000D),
            (short) (Math.Clamp(entity.Velocity.Z, -3.9D, 3.9D) * 8000D));
    }

    public void Write(Span<byte> span)
    {
        var writer = SpanWriter
            .Create(span)
            .WriteVariableInteger(unique)
            .WriteByte(type)
            .WriteInteger(x)
            .WriteInteger(y)
            .WriteInteger(z)
            .WriteByte(pitch)
            .WriteByte(yaw)
            .WriteInteger(data);

        if (data <= 0)
        {
            return;
        }

        writer
            .WriteShort(velocityX)
            .WriteShort(velocityY)
            .WriteShort(velocityZ);
    }
}