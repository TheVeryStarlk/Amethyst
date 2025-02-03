namespace Amethyst.Protocol;

internal interface IHandler
{
    public Task HandleAsync(Client client);
}