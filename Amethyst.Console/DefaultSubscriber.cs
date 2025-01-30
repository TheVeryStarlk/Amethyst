using Amethyst.Components;
using Amethyst.Components.Eventing;
using Amethyst.Components.Eventing.Sources.Client;
using Amethyst.Components.Messages;
using Microsoft.Extensions.Logging;

namespace Amethyst.Console;

internal sealed class DefaultSubscriber(ILogger<DefaultSubscriber> logger) : ISubscriber
{
    public void Subscribe(IRegistry registry)
    {
        logger.LogInformation("Registering events");

        registry.For<IClient>(consumer => consumer.On<StatusRequest>((_, request, _) =>
        {
            var description = Message
                .Create()
                .WriteLine("Hello, world!", true)
                .Write("Powered by ", color: Color.Gray)
                .Write("Amethyst", underlined: true, color: Color.LightPurple)
                .Write(".", color: Color.Gray)
                .Build();

            request.Status = Status.Create("Amethyst", 47, 0, 0, description, string.Empty);
            return Task.CompletedTask;
        }));
    }
}