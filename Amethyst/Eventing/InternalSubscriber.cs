using Amethyst.Entities;
using Amethyst.Eventing.Sources.Players;
using Amethyst.Protocol.Packets.Play;

namespace Amethyst.Eventing;

internal sealed class InternalSubscriber(EventDispatcher eventDispatcher) : ISubscriber
{
    public static void Create(EventDispatcher eventDispatcher, Registry registry)
    {
        var subscriber = new InternalSubscriber(eventDispatcher);
        subscriber.Subscribe(registry);
    }

    public void Subscribe(IRegistry registry)
    {
        registry.For<Player>(consumer => consumer.On<Received>(async (player, received, cancellationToken) =>
        {
            var packet = received.Packet;

            switch (packet.Identifier)
            {
                case 3:
                    var onGround = packet.Create<OnGroundPacket>();

                    await DispatchMovedAsync(
                        player,
                        player.X,
                        player.Y,
                        player.Z,
                        player.Yaw,
                        player.Pitch,
                        onGround.Value,
                        cancellationToken).ConfigureAwait(false);

                    player.OnGround = onGround.Value;
                    break;

                case 4:
                    var position = packet.Create<PositionPacket>();

                    await DispatchMovedAsync(
                        player,
                        position.X,
                        position.Y,
                        position.Z,
                        player.Yaw,
                        player.Pitch,
                        player.OnGround,
                        cancellationToken).ConfigureAwait(false);

                    player.X = position.X;
                    player.Y = position.Y;
                    player.Z = position.Z;
                    player.OnGround = position.OnGround;
                    break;

                case 5:
                    var look = packet.Create<LookPacket>();

                    await DispatchMovedAsync(
                        player,
                        player.X,
                        player.Y,
                        player.Z,
                        look.Yaw,
                        look.Pitch,
                        look.OnGround,
                        cancellationToken).ConfigureAwait(false);

                    player.Yaw = look.Yaw;
                    player.Pitch = look.Pitch;
                    player.OnGround = look.OnGround;

                    break;

                case 6:
                    var positionLook = packet.Create<PositionLookPacket>();

                    await DispatchMovedAsync(
                        player,
                        positionLook.X,
                        positionLook.Y,
                        positionLook.Z,
                        positionLook.Yaw,
                        positionLook.Pitch,
                        positionLook.OnGround,
                        cancellationToken).ConfigureAwait(false);

                    player.X = positionLook.X;
                    player.Y = positionLook.Y;
                    player.Z = positionLook.Z;
                    player.Yaw = positionLook.Yaw;
                    player.Pitch = positionLook.Pitch;
                    player.OnGround = positionLook.OnGround;

                    break;
            }

        }));
    }

    private async Task DispatchMovedAsync(
        Player player,
        double x,
        double y,
        double z,
        float yaw,
        float pitch,
        bool onGround,
        CancellationToken cancellationToken)
    {
        const double tolerance = 0.5f;

        if (Math.Abs(player.X - x) > tolerance
            && Math.Abs(player.Y - y) > tolerance
            && Math.Abs(player.Z - z) > tolerance
            && Math.Abs(player.Yaw - yaw) > tolerance
            && Math.Abs(player.Pitch - pitch) > tolerance
            && player.OnGround == onGround)
        {
            return;
        }

        var moved = new Moved(player.X, player.Y, player.Y, player.Yaw, player.Pitch, player.OnGround);
        await eventDispatcher.DispatchAsync(player, moved, cancellationToken).ConfigureAwait(false);
    }
}