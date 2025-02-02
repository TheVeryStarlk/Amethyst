using Amethyst.Components;
using Amethyst.Components.Eventing;
using Amethyst.Components.Eventing.Sources.Client;
using Amethyst.Components.Eventing.Sources.Server;
using Amethyst.Components.Messages;
using Amethyst.Protocol.Packets.Play;

namespace Amethyst.Console;

internal sealed class DefaultSubscriber : ISubscriber
{
    private IServer? server;

    public void Subscribe(IRegistry registry)
    {
        registry.For<IServer>(consumer =>
        {
            consumer.On<Starting>((source, _, _) =>
            {
                server = source;
                return Task.CompletedTask;
            });

            consumer.On<Stopping>((_, stopping, _) =>
            {
                stopping.Message = Message.Create("Come back later!");
                return Task.CompletedTask;
            });
        });

        registry.For<IClient>(consumer =>
        {
            consumer.On<Outdated>((_, outdated, _) =>
            {
                // Eh, should probably cache this.
                var message = Message
                    .Create()
                    .Write("You're scaring me!").Red()
                    .Build();

                outdated.Message = message;
                return Task.CompletedTask;
            });

            consumer.On<ReceivedPacket>((_, received, _) =>
            {
                if (received.Packet.Identifier == MessagePacket.Identifier)
                {
                    server?.Stop();
                }

                return Task.CompletedTask;
            });

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