using System.Diagnostics;
using Amethyst.Abstractions;
using Amethyst.Abstractions.Worlds;
using Amethyst.Eventing;
using Amethyst.Eventing.Client;
using Microsoft.Extensions.Logging;

namespace Amethyst.BlockParty;

internal sealed class Subscriber(ILogger<Subscriber> logger, IWorldFactory worldFactory) : ISubscriber
{
    public void Subscribe(IRegistry registry)
    {
        var world = worldFactory.Create("Main", EmptyGenerator.Instance);

        logger.LogInformation("Started converting the Anvil world");

        var watch = Stopwatch.StartNew();
        var count = Anvil.Load("Regions", world);

        logger.LogInformation("Converted {Count} regions in {Milliseconds} milliseconds", count, watch.ElapsedMilliseconds);

        registry.For<IClient>(consumer => consumer.On<Joining>((_, original) => original.World = world));
    }
}