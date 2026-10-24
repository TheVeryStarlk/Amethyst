using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Eventing.Player;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Extensions.Subscribers;

/// <summary>
/// Applies the blocks players break and place to their world, and shows them to the other players.
/// </summary>
public sealed class BlockSubscriber : ISubscriber
{
    public void Subscribe(IRegistry registry)
    {
        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Dig>((source, dig) =>
            {
                // Creative clients break blocks instantly, so they never send a finished status.
                var broken = dig.Status is Digging.Finished || source.GameMode is GameMode.Creative && dig.Status is Digging.Started;

                if (broken)
                {
                    source.World.Set(dig.Position, Blocks.Air, source);
                }
            });

            consumer.On<Placed>((source, placed) =>
            {
                source.World.Set(placed.Position, placed.Block, source);

                // Clients do not play the sound of blocks they place, so the source hears it from the server too.
                var packet = Place(placed.Position, placed.Block);

                foreach (var pair in source.World.Players)
                {
                    pair.Value.Client.Write(packet);
                }
            });
        });
    }

    private static SoundPacket Place(Position position, Block block)
    {
        // The name, volume and pitch of each block's sound type.
        var (name, volume, pitch) = block.Type switch
        {
            5 or 17 or 25 or 47 or 50 or 53 or 54 or 58 or 65 or 69 or 72 or 75 or 76 or 85 or 86 or 91 or 96 or 99 or 100
                or 103 or 107 or 125 or 126 or 134 or 135 or 136 or 143 or 146 or 147 or 148 or 151 or 162 or 163 or 164
                or >= 183 and <= 192 => ("dig.wood", 1F, 1F),

            2 or 6 or 18 or 19 or 31 or 32 or 37 or 38 or 39 or 40 or 46 or 106 or 110 or 111 or 161 or 170 or 175 => ("dig.grass", 1F, 1F),

            3 or 13 or 60 or 82 => ("dig.gravel", 1F, 1F),

            12 or 88 => ("dig.sand", 1F, 1F),

            35 or 81 or 171 => ("dig.cloth", 1F, 1F),

            78 or 80 => ("dig.snow", 1F, 1F),

            20 or 79 or 89 or 95 or 102 or 120 or 123 or 124 or 160 or 169 or 174 => ("step.stone", 1F, 1F),

            27 or 28 or 41 or 42 or 52 or 57 or 66 or 101 or 133 or 152 or 154 or 157 or 167 => ("dig.stone", 1F, 1.5F),

            145 => ("random.anvil_land", 0.3F, 1F),

            165 => ("mob.slime.big", 1F, 1F),

            _ => ("dig.stone", 1F, 1F)
        };

        // Placing plays the sound from the block's center, a bit quieter and lower than the sound type itself.
        var center = new Position(position.X + 0.5D, position.Y + 0.5D, position.Z + 0.5D);

        return new SoundPacket(name, center, (volume + 1F) / 2F, pitch * 0.8F);
    }
}