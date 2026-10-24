using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Eventing.Player;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Worlds;
using Amethyst.Extensions;

namespace Amethyst.Example.Subscribers;

/// <summary>
/// Relays chat between every world, giving each player their own name color.
/// </summary>
internal sealed class ChatSubscriber(IWorldFactory worldFactory) : ISubscriber
{
    private static readonly Color[] colors = [Color.Aqua, Color.Gold, Color.Green, Color.LightPurple, Color.Yellow, Color.Blue, Color.Red];

    public void Subscribe(IRegistry registry)
    {
        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Joined>((player, _) =>
            {
                player.Send(Chat.Of(
                    "Welcome to the expedition, ".Paint(Color.Gray),
                    player.Username.Paint(Tint(player)),
                    "! Your hotbar holds four wands, type ".Paint(Color.Gray),
                    "/help".Paint(Color.LightPurple),
                    " to see what else you can do.".Paint(Color.Gray)));

                worldFactory.Broadcast(Chat.Of("+ ".Paint(Color.Green, true), player.Username.Paint(Tint(player)), " set out on an expedition.".Paint(Color.Gray)));
            });

            consumer.On<Left>((player, _) =>
            {
                worldFactory.Broadcast(Chat.Of("- ".Paint(Color.Red, true), player.Username.Paint(Tint(player)), " returned home.".Paint(Color.Gray)));
            });

            consumer.On<Sent>((player, sent) =>
            {
                // Commands are handled by the command subscriber.
                if (sent.Message.StartsWith('/'))
                {
                    return;
                }

                worldFactory.Broadcast(Chat.Of(player.Username.Paint(Tint(player)), " > ".Paint(Color.DarkGray), sent.Message.Paint(Color.White)));
            });
        });
    }

    /// <summary>
    /// The same username always gets the same color.
    /// </summary>
    public static Color Tint(IPlayer player)
    {
        var hash = 0;

        // Not string.GetHashCode, which changes between runs.
        foreach (var character in player.Username)
        {
            hash = hash * 31 + character;
        }

        return colors[(hash & int.MaxValue) % colors.Length];
    }
}
