using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Networking.Packets.Login;
using Amethyst.Abstractions.Networking.Packets.Play;
using Amethyst.Eventing;
using Amethyst.Eventing.Client;
using Amethyst.Eventing.Player;
using Amethyst.Eventing.Server;

namespace Amethyst.Hosting.Subscribers;

internal sealed class PlayerSubscriber : ISubscriber
{
    private readonly Dictionary<string, IPlayer> pairs = [];
    private readonly FailurePacket failure = new(Message.Simple("Already logged in"));

    private DateTime last;

    public void Subscribe(IRegistry registry)
    {
        registry.For<IClient>(consumer => consumer.On<Login>((source, original) =>
        {
            if (pairs.ContainsKey(original.Username))
            {
                source.Write(failure);
            }
        }));

        registry.For<IPlayer>(consumer => consumer.On<Joined>((source, _) => pairs[source.Username] = source));
        registry.For<IPlayer>(consumer => consumer.On<Left>((source, _) => pairs.Remove(source.Username)));

        registry.For<IServer>(consumer => consumer.On<Tick>((_, _) =>
        {
            var now = DateTime.Now;

            // Should keep alive interval be configurable?
            if (now - last < TimeSpan.FromSeconds(5))
            {
                return;
            }

            last = now;

            var alive = new KeepAlivePacket(now.Ticks);

            foreach (var pair in pairs)
            {
                pair.Value.Client.Write(alive);
            }
        }));
    }
}