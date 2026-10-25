using System.Collections.Concurrent;
using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Eventing.Client;
using Amethyst.Abstractions.Eventing.Player;
using Amethyst.Abstractions.Messages;

namespace Amethyst.Extensions.Subscribers;

/// <summary>
/// Rejects players joining with a username already taken in any world, ignoring case.
/// Without it, usernames are only unique within a world.
/// </summary>
public sealed class UsernameSubscriber : ISubscriber
{
    // Scanning the worlds then joining would let two clients take the same username at once, so it is claimed in one step.
    // Each username remembers the client that claimed it, so a release never frees a newer client's claim.
    private readonly ConcurrentDictionary<string, IClient> usernames = new(StringComparer.OrdinalIgnoreCase);

    public void Subscribe(IRegistry registry)
    {
        registry.For<IClient>(consumer =>
        {
            consumer.On<Joining>((source, joining) =>
            {
                if (usernames.TryAdd(joining.Username, source))
                {
                    return;
                }

                joining.Message = Message.Simple("Username already taken!");
                joining.IsCancelled = true;
            });

            // A later subscriber or the server can still reject the client after it claimed the username.
            consumer.On<Rejected>((source, rejected) => Release(rejected.Username, source));
        });

        registry.For<IPlayer>(consumer => consumer.On<Left>((source, _) => Release(source.Username, source.Client)));
    }

    private void Release(string username, IClient client)
    {
        usernames.TryRemove(new KeyValuePair<string, IClient>(username, client));
    }
}