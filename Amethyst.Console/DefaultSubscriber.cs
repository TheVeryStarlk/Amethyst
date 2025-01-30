using Amethyst.Components;
using Amethyst.Components.Eventing;
using Amethyst.Components.Eventing.Sources.Client;
using Microsoft.Extensions.Logging;

namespace Amethyst.Console;

internal sealed class DefaultSubscriber(ILogger<DefaultSubscriber> logger) : ISubscriber
{
    public void Subscribe(IRegistry registry)
    {
        logger.LogInformation("Registering events");

        registry.For<IClient>(consumer => consumer.On<Received>((_, received, _) =>
        {
            logger.LogInformation("Received {Identifier}", received.Message.Identifier);
            return Task.CompletedTask;
        }));
    }
}