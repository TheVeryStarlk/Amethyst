namespace Amethyst.Eventing.Sources.Clients;

public sealed class Login(string username) : Event<Client>
{
    public string Username => username;
}