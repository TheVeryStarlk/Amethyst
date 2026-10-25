using System.Net.Sockets;
using Amethyst.Eventing;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class ClientFactory(ILogger<Client> logger, Dispatcher dispatcher)
{
    public Client Create(Socket socket)
    {
        return new Client(logger, dispatcher, socket);
    }
}