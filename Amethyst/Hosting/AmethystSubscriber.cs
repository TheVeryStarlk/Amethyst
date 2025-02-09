using Amethyst.Components.Entities;
using Amethyst.Components.Eventing;
using Amethyst.Components.Eventing.Sources.Players;
using Amethyst.Entities;

namespace Amethyst.Hosting;

internal sealed class AmethystSubscriber : ISubscriber
{
    public void Subscribe(IRegistry registry)
    {
        registry.For<IPlayer>(consumer => consumer.On<Moved>((source, moved, _) =>
        {
            var player = (Player) source;
            player.X = moved.X;
            player.Y = moved.Y;
            player.Z = moved.Z;
            player.Yaw = moved.Yaw;
            player.Pitch = moved.Pitch;
            player.OnGround = moved.OnGround;

            return Task.CompletedTask;
        }));
    }
}