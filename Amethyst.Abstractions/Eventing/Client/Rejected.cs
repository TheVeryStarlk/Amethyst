using Amethyst.Abstractions;

namespace Amethyst.Abstractions.Eventing.Client;

/// <summary>
/// Dispatched when a client did not join after <see cref="Joining"/>, whether it was cancelled or failed afterward.
/// </summary>
/// <remarks>
/// Cancelling <see cref="Joining"/> skips the remaining subscribers, so this is where to undo anything done on it.
/// </remarks>
public sealed class Rejected(string username) : IEvent<IClient>
{
    public string Username => username;
}