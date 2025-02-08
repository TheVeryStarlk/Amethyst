using Amethyst.Components;

namespace Amethyst;

internal sealed class Server : IServer
{
    private readonly CancellationTokenSource source = new();

    public void Stop()
    {
        source.Cancel();
    }
}