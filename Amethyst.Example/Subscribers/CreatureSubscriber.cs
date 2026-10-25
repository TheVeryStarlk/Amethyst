using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Eventing.Player;
using Amethyst.Abstractions.Eventing.Server;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Worlds;
using Amethyst.Example.Creatures;
using Amethyst.Extensions;

namespace Amethyst.Example.Subscribers;

/// <summary>
/// Brings the entity toys to life, opens the spawn plaza and sends pets home with their owners.
/// </summary>
internal sealed class CreatureSubscriber(Bestiary bestiary, Ballistics ballistics, Showcase showcase, Pets pets) : ISubscriber
{
    public void Subscribe(IRegistry registry)
    {
        // The world exists before anyone joins, so the plaza is ready for the first explorer.
        registry.For<IServer>(consumer => consumer.On<Starting>((_, _) => showcase.Plaza()));

        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Use>((player, use) =>
            {
                // Right-clicking the air reports a position of -1, -1, -1.
                var block = use.Position.Y is >= 0 and < 256;

                switch ((ItemType) use.Item.Type)
                {
                    case ItemType.GoldHoe:
                        Storm(player, block ? use.Position : null);
                        break;

                    case ItemType.Fireball:
                        Hurl(player);
                        break;

                    case ItemType.Bow:
                        Volley(player);
                        break;

                    case ItemType.SnowBall:
                        Flurry(player);
                        break;

                    case ItemType.MonsterEgg when block:
                        Hatch(player, use.Position, use.Item.Durability);
                        break;
                }
            });

            consumer.On<Placed>((player, placed) =>
            {
                if (placed.Block.Type != Blocks.Explosive.Type)
                {
                    return;
                }

                // TNT lights itself instead of being placed, the client is sent the air back.
                placed.IsCancelled = true;
                ballistics.Prime(player.World, placed.Position with { X = placed.Position.X + 0.5, Z = placed.Position.Z + 0.5 });
            });

            consumer.On<Left>((player, _) => pets.Dismiss(player));
        });
    }

    private void Storm(IPlayer player, Position? clicked)
    {
        var target = clicked is { } position
            ? position with { X = position.X + 0.5, Y = position.Y + 1, Z = position.Z + 0.5 }
            : Motion.Trace(player.World, Motion.Eye(player), Motion.Direction(player.Yaw, player.Pitch), 128);

        if (target is not { } strike)
        {
            player.Send("The storm cannot reach that far.".Paint(Color.Red), MessagePosition.HotBar);
            return;
        }

        ballistics.Strike(player.World, strike);
    }

    private void Hurl(IPlayer player)
    {
        var direction = Motion.Direction(player.Yaw, player.Pitch);

        ballistics.Fire<ILargeFireball>(player.World, Motion.Along(Motion.Eye(player), direction, 1.5), direction, player, 3.5);
        Motion.Effect(player.World, Effects.GhastFireball, player.Position);
    }

    private void Volley(IPlayer player)
    {
        var eye = Motion.Eye(player);

        // A fan of three arrows.
        foreach (var spread in (int[]) [-6, 0, 6])
        {
            var direction = Motion.Direction(player.Yaw + spread, player.Pitch);
            ballistics.Shoot(player.World, Motion.Along(eye, direction, 1), direction, player);
        }

        Motion.Effect(player.World, Effects.Bow, player.Position);
    }

    private void Flurry(IPlayer player)
    {
        var eye = Motion.Eye(player);

        for (var index = 0; index < 8; index++)
        {
            var direction = Motion.Direction(player.Yaw + Random.Shared.Next(-10, 11), player.Pitch + Random.Shared.Next(-10, 11));
            ballistics.Throw<ISnowball>(player.World, Motion.Along(eye, direction, 1), direction, 1.2 + Random.Shared.NextDouble() * 0.6);
        }

        Motion.Sound(player.World, Sounds.RandomBow, player.Position, 0.5F, 0.4F);
    }

    // Spawn eggs from the creative inventory hatch their mob, the mystery egg any of them.
    private void Hatch(IPlayer player, Position clicked, short durability)
    {
        var name = durability is 0 ? Bestiary.Pick(bestiary.Mobs) : Bestiary.Egg(durability);

        if (name is null)
        {
            return;
        }

        var position = clicked with { X = clicked.X + 0.5, Y = clicked.Y + 1, Z = clicked.Z + 0.5 };
        var yaw = Motion.Yaw(position.X - player.Position.X, position.Z - player.Position.Z);

        bestiary.Summon(name, new Summoning(player.World, position, yaw, 0, player));
        Motion.Effect(player.World, Effects.MobSpawn, position);

        player.Send(Chat.Of("A ".Paint(Color.Gray), Bestiary.Humanize(name).Paint(Color.Green), " hatched!".Paint(Color.Gray)), MessagePosition.HotBar);
    }
}
