using System.Buffers;
using System.Diagnostics;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Mobs;
using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Networking;
using Raspite;
using Raspite.Tags;

namespace Amethyst.Entities;

/// <summary>
/// The entries clients read an entity's metadata from, written with the entity's spawn or metadata packet.
/// </summary>
internal sealed class Metadata
{
    // The end of the entries is marked by a single byte.
    public int Length => entries.Sum(entry => sizeof(byte) + entry.Value switch
    {
        byte => sizeof(byte),
        short => sizeof(short),
        int => sizeof(int),
        float => sizeof(float),
        string value => Variable.GetByteCount(value),
        byte[] value => value.Length,
        EulerAngle => sizeof(float) * 3,
        _ => throw new UnreachableException()
    }) + sizeof(byte);

    // Keys hold the entry's type in their upper 3 bits and its index in their lower 5 bits.
    private readonly List<(byte Key, object Value)> entries = [];

    public static Metadata Create(ISpawnable entity)
    {
        var metadata = new Metadata();

        var flags = (entity.IsOnFire ? 1 : 0)
                    | (entity.IsCrouched ? 2 : 0)
                    | (entity.IsSprinting ? 8 : 0)
                    | (entity.IsEating ? 16 : 0)
                    | (entity.IsInvisible ? 32 : 0);

        metadata
            .Add(0, (byte) flags)
            .Add(1, entity.Air)
            .Add(2, entity.CustomName ?? string.Empty)
            .Add(3, entity.IsCustomNameVisible)
            .Add(4, entity.IsSilent);

        if (entity is ILiving living)
        {
            metadata
                .Add(6, living.Health)
                .Add(7, living.PotionColor)
                .Add(8, living.IsPotionAmbient)
                .Add(9, living.Arrows);
        }

        if (entity is IMob mob)
        {
            metadata.Add(15, !mob.HasAi);
        }

        if (entity is IAgeable ageable)
        {
            // Children have a negative age.
            metadata.Add(12, ageable.IsChild ? byte.MaxValue : (byte) 0);
        }

        if (entity is ITameable tameable)
        {
            // Wolves keep whether they are angry with the other flags.
            var taming = (tameable.IsSitting ? 1 : 0)
                        | (tameable is IWolf { IsAngry: true } ? 2 : 0)
                        | (tameable.IsTamed ? 4 : 0);

            metadata
                .Add(16, (byte) taming)
                .Add(17, tameable.Owner?.ToString() ?? string.Empty);
        }

        if (entity is IMinecart minecart)
        {
            // Unlike in chunks, the block's state here keeps the metadata in the upper bits.
            var block = minecart.DisplayedBlock.GetValueOrDefault();

            metadata
                .Add(17, minecart.ShakingPower)
                .Add(18, minecart.ShakingDirection)
                .Add(19, minecart.Damage)
                .Add(20, block.Type | (block.Metadata & 15) << 12)
                .Add(21, minecart.DisplayedOffset)
                .Add(22, minecart.DisplayedBlock.HasValue);
        }

        switch (entity)
        {
            case IArmorStand stand:
                var options = (stand.IsSmall ? 1 : 0)
                            | (stand.HasGravity ? 0 : 2)
                            | (stand.HasArms ? 4 : 0)
                            | (stand.HasBasePlate ? 0 : 8)
                            | (stand.IsMarker ? 16 : 0);

                metadata
                    .Add(10, (byte) options)
                    .Add(11, stand.Head)
                    .Add(12, stand.Body)
                    .Add(13, stand.LeftArm)
                    .Add(14, stand.RightArm)
                    .Add(15, stand.LeftLeg)
                    .Add(16, stand.RightLeg);

                break;

            case IArrow arrow:
                metadata.Add(16, arrow.IsCritical);
                break;

            case IBat bat:
                metadata.Add(16, bat.IsHanging);
                break;

            case IBlaze blaze:
                metadata.Add(16, blaze.IsCharging);
                break;

            case IBoat boat:
                metadata
                    .Add(17, boat.TimeSinceHit)
                    .Add(18, boat.ForwardDirection)
                    .Add(19, boat.Damage);

                break;

            case ICommandBlockMinecart command:
                metadata
                    .Add(23, command.Command)
                    .Add(24, command.LastOutput?.Serialize() ?? string.Empty);

                break;

            case ICreeper creeper:
                // Idle creepers have a negative state.
                metadata
                    .Add(16, creeper.IsFusing ? (byte) 1 : byte.MaxValue)
                    .Add(17, creeper.IsPowered);

                break;

            case IDroppedItem item:
                metadata.Add(10, item.Item);
                break;

            case IEnderman enderman:
                // Unlike in chunks, the block's state here keeps the metadata in the upper bits.
                metadata
                    .Add(16, (short) (enderman.Carried.Type | (enderman.Carried.Metadata & 15) << 12))
                    .Add(18, enderman.IsScreaming);

                break;

            case IFirework firework:
                metadata.Add(8, firework.Item);
                break;

            case IFurnaceMinecart furnace:
                metadata.Add(16, furnace.IsPowered);
                break;

            case IGhast ghast:
                metadata.Add(16, ghast.IsAttacking);
                break;

            case IGuardian guardian:
                metadata
                    .Add(16, (guardian.IsRetractingSpikes ? 2 : 0) | (guardian.IsElder ? 4 : 0))
                    .Add(17, guardian.Target?.Unique ?? 0);

                break;

            case IHorse horse:
                var traits = (horse.IsTamed ? 2 : 0)
                             | (horse.HasSaddle ? 4 : 0)
                             | (horse.HasChest ? 8 : 0)
                             | (horse.IsBred ? 16 : 0)
                             | (horse.IsGrazing ? 32 : 0)
                             | (horse.IsRearing ? 64 : 0)
                             | (horse.IsMouthOpen ? 128 : 0);

                metadata
                    .Add(16, traits)
                    .Add(19, (byte) horse.Type)
                    .Add(20, (int) horse.Color | (int) horse.Markings << 8)
                    .Add(21, horse.Owner?.ToString() ?? string.Empty)
                    .Add(22, (int) horse.Armor);

                break;

            case IIronGolem golem:
                metadata.Add(16, golem.IsPlayerCreated);
                break;

            case IItemFrame frame:
                metadata
                    .Add(8, frame.Item)
                    .Add(9, frame.Rotation);

                break;

            case IOcelot ocelot:
                metadata.Add(18, (byte) ocelot.Type);
                break;

            case IPig pig:
                metadata.Add(16, pig.HasSaddle);
                break;

            case IRabbit rabbit:
                metadata.Add(18, (byte) rabbit.Type);
                break;

            case ISheep sheep:
                metadata.Add(16, (byte) ((int) sheep.Color & 15 | (sheep.IsSheared ? 16 : 0)));
                break;

            case ISkeleton skeleton:
                metadata.Add(13, (byte) skeleton.Type);
                break;

            case ISlime slime:
                metadata.Add(16, slime.Size);
                break;

            case ISpider spider:
                metadata.Add(16, spider.IsClimbing);
                break;

            case IVillager villager:
                metadata.Add(16, (int) villager.Profession);
                break;

            case IWitch witch:
                metadata.Add(21, witch.IsAggressive);
                break;

            case IWither wither:
                metadata
                    .Add(17, wither.CenterTarget?.Unique ?? 0)
                    .Add(18, wither.RightTarget?.Unique ?? 0)
                    .Add(19, wither.LeftTarget?.Unique ?? 0)
                    .Add(20, wither.InvulnerableTime);

                break;

            case IWitherSkull skull:
                metadata.Add(10, skull.IsCharged);
                break;

            case IWolf wolf:
                // The tail follows the health, and collars use dye colors which are in the reverse order of wool.
                metadata
                    .Add(18, wolf.Health)
                    .Add(19, wolf.IsBegging)
                    .Add(20, (byte) (15 - (int) wolf.CollarColor));

                break;

            case IZombie zombie:
                metadata
                    .Add(12, zombie.IsChild)
                    .Add(13, zombie.IsVillager)
                    .Add(14, zombie.IsConverting);

                break;
        }

        return metadata;
    }

    public void Write(ref SpanWriter writer)
    {
        foreach (var (key, value) in entries)
        {
            writer.WriteByte(key);

            switch (value)
            {
                case byte current:
                    writer.WriteByte(current);
                    break;

                case short current:
                    writer.WriteShort(current);
                    break;

                case int current:
                    writer.WriteInteger(current);
                    break;

                case float current:
                    writer.WriteFloat(current);
                    break;

                case string current:
                    writer.WriteVariableString(current);
                    break;

                case byte[] current:
                    writer.Write(current);
                    break;

                // Chained calls write through copies, which would leave the writer behind.
                case EulerAngle current:
                    writer.WriteFloat(current.X);
                    writer.WriteFloat(current.Y);
                    writer.WriteFloat(current.Z);
                    break;

                default:
                    throw new UnreachableException();
            }
        }

        writer.WriteByte(127);
    }

    private Metadata Add(byte index, byte value)
    {
        return Add(0, index, value);
    }

    private Metadata Add(byte index, bool value)
    {
        return Add(0, index, value ? (byte) 1 : (byte) 0);
    }

    private Metadata Add(byte index, short value)
    {
        return Add(1, index, value);
    }

    private Metadata Add(byte index, int value)
    {
        return Add(2, index, value);
    }

    private Metadata Add(byte index, float value)
    {
        return Add(3, index, value);
    }

    private Metadata Add(byte index, string value)
    {
        return Add(4, index, value);
    }

    private Metadata Add(byte index, Item value)
    {
        // Empty slots are only written as a type of -1.
        if (value.IsEmpty)
        {
            var empty = new byte[sizeof(short)];
            SpanWriter.Create(empty).WriteShort(-1);

            return Add(5, index, empty);
        }

        // Items without NBT are written as a single end tag.
        byte[] tag = [Tag.End];

        if (value.Tag is not null)
        {
            var buffer = new ArrayBufferWriter<byte>();

            TagSerializer.Serialize(buffer, value.Tag);

            tag = [.. buffer.WrittenSpan];
        }

        var slot = new byte[sizeof(short) + sizeof(byte) + sizeof(short) + tag.Length];

        SpanWriter
            .Create(slot)
            .WriteShort(value.Type)
            .WriteByte(value.Amount)
            .WriteShort(value.Durability)
            .Write(tag);

        return Add(5, index, slot);
    }

    private Metadata Add(byte index, EulerAngle value)
    {
        return Add(7, index, value);
    }

    private Metadata Add(byte type, byte index, object value)
    {
        entries.Add(((byte) (type << 5 | index & 31), value));
        return this;
    }
}