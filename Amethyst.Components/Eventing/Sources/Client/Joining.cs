namespace Amethyst.Components.Eventing.Sources.Client;

public sealed class Joining(string username) : Event<IClient>
{
    public string Username => username;
}