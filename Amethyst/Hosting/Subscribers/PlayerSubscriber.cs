using Amethyst.Abstractions;
using Amethyst.Abstractions.Networking.Packets.Play;
using Amethyst.Eventing;
using Amethyst.Eventing.Server;

namespace Amethyst.Hosting.Subscribers;

internal sealed class PlayerSubscriber : ISubscriber
{
    private DateTime last;

    public void Subscribe(IRegistry registry)
    {
        registry.For<IServer>(consumer => consumer.On<Tick>((source, _) =>
        {
            var now = DateTime.Now;

            // Should keep alive interval be configurable?
            if (now - last < TimeSpan.FromSeconds(5))
            {
                return;
            }

            last = now;

            var packet = new KeepAlivePacket(now.Ticks);

            foreach (var world in source.Worlds)
            {
                foreach (var player in world.Players)
                {
                    player.Client.Write(packet);
                }
            }
        }));
    }
}