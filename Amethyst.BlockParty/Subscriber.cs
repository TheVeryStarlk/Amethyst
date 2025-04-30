using System.Diagnostics;
using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Abstractions.Worlds;
using Amethyst.Eventing;
using Amethyst.Eventing.Client;
using Amethyst.Eventing.Player;
using Microsoft.Extensions.Logging;

namespace Amethyst.BlockParty;

internal sealed class Subscriber(ILogger<Subscriber> logger, IWorldFactory worldFactory) : ISubscriber
{
    private const string Favicon = "data:image/png;base64,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";

    private readonly IWorld world = worldFactory.Create("Main", EmptyGenerator.Instance);

    private State state;
    private Block block;

    public void Subscribe(IRegistry registry)
    {
        logger.LogInformation("Started converting the Anvil world");

        var watch = Stopwatch.StartNew();
        var count = Anvil.Load("Regions", world);

        logger.LogInformation("Converted {Count} regions in {Milliseconds} milliseconds", count, watch.ElapsedMilliseconds);

        registry.For<IClient>(consumer =>
        {
            consumer.On<Request>((_, original) =>
            {
                original.Name = "Sloth";
                original.Maximum = 5;
                original.Online = world.Players.Count;

                var description = Message
                    .Create()
                    .Write("     ")
                    .Write("   ").White().Obfuscated()
                    .Write(" This is Sloth's server ").DarkGreen().Bold()
                    .Write("   ").White().Obfuscated()
                    .WriteLine()
                    .Write("Do not believe anything Sloth says").Red().Bold()
                    .Build();

                original.Description = description;
                original.Favicon = Favicon;
            });

            consumer.On<Joining>((_, original) =>
            {
                original.GameMode = GameMode.Creative;
                original.World = world;
            });
        });

        registry.For<IPlayer>(consumer => consumer.On<Joined>((source, _) =>
        {
            source.Teleport(new Position(0, 16, 0));

            Start();

            if (world.Players.Count > 5)
            {
                var message = Message
                    .Create()
                    .Write("Come later!").Red()
                    .Build();

                source.Client.Write(new DisconnectPacket(message));
                source.Client.Stop();

                return;
            }

            if (state is State.Waiting)
            {
                if (world.Players.Count > 1)
                {
                    state = State.Started;
                }

                source.Send(Message.Create().Write("Need at least two players...").Yellow().Build());
                return;
            }

            if (state is State.Started)
            {
                source.Send(Message.Create().Write("Match has already started.").Yellow().Build());
                return;
            }
        }));
    }

    private void Start()
    {
        for (var x = -10; x <= 10; x++)
        {
            for (var z = -10; z <= 10; z++)
            {
                block = new Block(159, Random.Shared.Next(15));
                world[x, 0, z] = block;

                foreach (var pair in world.Players)
                {
                    var packet = new BlockPacket(new Position(x, 0, z), block);
                    pair.Value.Client.Write(packet);
                }
            }
        }

        logger.LogInformation("Built the floor. Chosen block is {Number}", block.Metadata);
    }
}

internal enum State
{
    Waiting,
    Started,
    Playing
}