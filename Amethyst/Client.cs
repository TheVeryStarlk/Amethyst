using Amethyst.Api.Entities;
using Amethyst.Protocol;
using Microsoft.AspNetCore.Connections;

namespace Amethyst;

internal sealed class Client(ConnectionContext connectionContext) : IClient, IAsyncDisposable
{
    public IPlayer? Player { get; }

    public void Queue(IOutgoingPacket packet)
    {
        throw new NotImplementedException();
    }

    public void Stop()
    {
        throw new NotImplementedException();
    }

    public async ValueTask DisposeAsync()
    {
        await connectionContext.DisposeAsync();
    }
}

internal interface IClient
{
    public IPlayer? Player { get; }

    public void Queue(IOutgoingPacket packet);

    public void Stop();
}