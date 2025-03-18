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
    private readonly Dictionary<string, IPlayer> players = [];
    private readonly FailurePacket packet = new(Message.Simple("Already logged in"));

    private DateTime last;

    public void Subscribe(IRegistry registry)
    {
        registry.For<IClient>(consumer => consumer.On<Login>((source, original) =>
        {
            if (players.ContainsKey(original.Username))
            {
                source.Write(packet);
            }
        }));

        registry.For<IPlayer>(consumer => consumer.On<Joined>((source, _) => players[source.Username] = source));

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
        }));
    }
}