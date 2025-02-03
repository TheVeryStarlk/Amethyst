using Amethyst.Abstractions;

namespace Amethyst.Eventing.Sources.Client;

public sealed class Login(string username) : Event<IClient>
{
    public string Username => username;
}