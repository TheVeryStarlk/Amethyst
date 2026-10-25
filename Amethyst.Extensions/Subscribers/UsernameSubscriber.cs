using Amethyst.Abstractions;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Eventing.Client;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Extensions.Subscribers;

/// <summary>
/// Rejects players joining with a username already taken in any world, ignoring case.
/// Without it, usernames are only unique within a world.
/// </summary>
public sealed class UsernameSubscriber(IWorldFactory worldFactory) : ISubscriber
{
    public void Subscribe(IRegistry registry)
    {
        registry.For<IClient>(consumer => consumer.On<Joining>((_, joining) =>
        {
            // Not atomic with the join, two players logging in at the same instant into different worlds can both get in.
            if (!worldFactory.Worlds.Any(world => world.Players.ContainsKey(joining.Username)))
            {
                return;
            }

            joining.Message = Message.Simple("Username already taken!");
            joining.IsCancelled = true;
        }));
    }
}