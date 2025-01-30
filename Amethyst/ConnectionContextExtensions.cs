using Amethyst.Protocol;
using Microsoft.AspNetCore.Connections;

namespace Amethyst;

internal static class ConnectionContextExtensions
{
    public static (ProtocolReader Input, ProtocolWriter Output) CreateProtocol(this ConnectionContext connectionContext)
    {
        return (new ProtocolReader(connectionContext.Transport.Input), new ProtocolWriter(connectionContext.Transport.Output));
    }
}