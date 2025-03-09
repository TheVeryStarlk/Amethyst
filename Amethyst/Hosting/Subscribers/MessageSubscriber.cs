using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Eventing.Players;
using Amethyst.Eventing;
using Amethyst.Protocol.Packets.Play;

namespace Amethyst.Hosting.Subscribers;

internal sealed class MessageSubscriber : ISubscriber
{
    public void Subscribe(Registry registry)
    {
        registry.For<IPlayer>(consumer => consumer.On<Tab>((source, original) =>
        {
            // This is not a complete implementation of the tab feature.
            source.Client.Write(original.Behind.Contains(' ')
                ? new TabResponsePacket(original.Matches)
                : new TabResponsePacket(original.Matches.Select(match => $"/{match}").ToArray()));
        }));
    }
}