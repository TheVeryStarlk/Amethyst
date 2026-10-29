using System.Net.Sockets;
using Amethyst.Eventing;
using Amethyst.Networking.Ingoing;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class ClientFactory(ILoggerFactory loggerFactory, Dispatcher dispatcher, Router router)
{
    public Client Create(Socket socket)
    {
        return new Client(loggerFactory.CreateLogger<Client>(), dispatcher, router, socket);
    }
}