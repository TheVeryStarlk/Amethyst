using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Abstractions.Eventing.Client;

public sealed class Joining(string username) : IEvent<IClient>, ICancellable
{
    public string Username => username;

    public IWorld? World { get; set; }

    public GameMode GameMode { get; set; }

    /// <summary>
    /// Whether the connection is encrypted. Before accepting, the client authenticates its session with Mojang,
    /// so offline clients cannot join, but the server does not verify that session.
    /// </summary>
    public bool Encrypted { get; set; }

    /// <summary>
    /// The reason sent to the client if the event was cancelled.
    /// </summary>
    public Message Message { get; set; } = Message.Simple("Disconnected!");

    public bool IsCancelled { get; set; }
}