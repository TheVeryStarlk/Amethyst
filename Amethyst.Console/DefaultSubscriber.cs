using Amethyst.Components;
using Amethyst.Components.Eventing;
using Amethyst.Components.Eventing.Sources.Client;
using Amethyst.Components.Eventing.Sources.Server;
using Amethyst.Components.Messages;
using Microsoft.Extensions.Logging;

namespace Amethyst.Console;

internal sealed class DefaultSubscriber(ILogger<DefaultSubscriber> logger) : ISubscriber
{
    public void Subscribe(IRegistry registry)
    {
        logger.LogInformation("Registering events");

        registry.For<IServer>(consumer => consumer.On<Stopping>((_, stopping, _) =>
        {
            stopping.Message = Message.Create("Come back later!");
            return Task.CompletedTask;
        }));

        registry.For<IClient>(consumer =>
        {
            consumer.On<Joining>((_, joining, _) =>
            {
                joining.GameMode = 0;
                return Task.CompletedTask;
            });

            consumer.On<StatusRequest>((_, request, _) =>
            {
                var description = Message
                    .Create()
                    .WriteLine("Hello, world!").Bold()
                    .Write("Powered by ").Gray()
                    .Write("Amethyst").LightPurple()
                    .Build();

                request.Status = Status.Create("Amethyst", 47, 1, 0, description, string.Empty);
                return Task.CompletedTask;
            });
        });
    }
}