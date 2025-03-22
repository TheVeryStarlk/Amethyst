using Amethyst.Abstractions;
using Amethyst.Abstractions.Worlds;
using Amethyst.Eventing;
using Amethyst.Worlds;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Server(ILoggerFactory loggerFactory, EventDispatcher eventDispatcher) : IServer
{
    private readonly ILogger<Server> logger = loggerFactory.CreateLogger<Server>();
    private readonly CancellationTokenSource source = new();

    public IWorld Create(string name, WorldType type, Dimension dimension, Difficulty difficulty, IGenerator generator)
    {
        return new World(name, type, dimension, difficulty, generator);
    }

    public void Stop()
    {
        source.Cancel();
    }
}