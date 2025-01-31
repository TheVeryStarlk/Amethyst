using Amethyst.Protocol;
using Microsoft.AspNetCore.Connections;

namespace Amethyst;

internal static class ConnectionContextExtensions
{
    public static ProtocolDuplex CreateProtocol(this ConnectionContext connectionContext)
    {
        return new ProtocolDuplex(new ProtocolReader(connectionContext.Transport.Input), new ProtocolWriter(connectionContext.Transport.Output));
    }
}

internal sealed record ProtocolDuplex(ProtocolReader Input, ProtocolWriter Output);