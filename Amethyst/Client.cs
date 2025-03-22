using System.IO.Pipelines;
using System.Net.Sockets;
using System.Threading.Channels;
using Amethyst.Abstractions;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Networking.Packets;
using Amethyst.Abstractions.Networking.Packets.Login;
using Amethyst.Abstractions.Networking.Packets.Play;
using Amethyst.Abstractions.Networking.Packets.Status;
using Amethyst.Entities;
using Amethyst.Eventing;
using Amethyst.Eventing.Client;
using Amethyst.Eventing.Player;
using Amethyst.Networking;
using Amethyst.Networking.Packets;
using Amethyst.Networking.Packets.Handshake;
using Amethyst.Networking.Packets.Login;
using Amethyst.Networking.Packets.Status;
using Amethyst.Networking.Serializers;
using Amethyst.Worlds;
using Microsoft.Extensions.Logging;

namespace Amethyst;

// Rewrite this when https://github.com/davidfowl/BedrockFramework/issues/172.
internal sealed class Client(ILogger<Client> logger, Socket socket, EventDispatcher eventDispatcher) : IClient, IDisposable
{
    private readonly NetworkStream stream = new(socket);
    private readonly CancellationTokenSource source = new();
    private readonly Channel<IOutgoingPacket> outgoing = Channel.CreateUnbounded<IOutgoingPacket>();

    private Player? player;
    private State state;

    public async Task StartAsync()
    {
        var reading = ReadingAsync();
        var writing = WritingAsync();

        // If reading finishes first, then either the client stopped the connection or the token was cancelled,
        // but there still is some work to do in the writing task, so wait for that work to finish.
        if (await Task.WhenAny(reading, writing).ConfigureAwait(false) == reading)
        {
            if (!source.IsCancellationRequested)
            {
                outgoing.Writer.Complete();
            }

            await writing.ConfigureAwait(false);
        }

        // Writing might finish before reading, so in that case cancelling the token will stop the reading task.
        source.Cancel();

        if (state is State.Play)
        {
            eventDispatcher.Dispatch(player!, new Left());
        }
    }

    public void Write(params ReadOnlySpan<IOutgoingPacket> packets)
    {
        if (source.IsCancellationRequested)
        {
            return;
        }

        foreach (var packet in packets)
        {
            // Writer complete is never called. Doesn't hurt to have this check, though.
            if (!outgoing.Writer.TryWrite(packet))
            {
                break;
            }
        }
    }

    public void Stop()
    {
        source.Cancel();
    }

    public void Dispose()
    {
        source.Dispose();
        socket.Dispose();
    }

    private async Task ReadingAsync()
    {
        var input = PipeReader.Create(stream);

        while (true)
        {
            try
            {
                var result = await input.ReadAsync(source.Token).ConfigureAwait(false);
                var sequence = result.Buffer;

                var consumed = sequence.Start;
                var examined = sequence.End;

                if (Protocol.TryRead(ref sequence, out var packet))
                {
                    Action<Packet> action = state switch
                    {
                        State.Handshake => Handshake,
                        State.Status => Status,
                        State.Login => Login,
                        State.Play => Play,
                        _ => throw new ArgumentOutOfRangeException()
                    };

                    action(packet);

                    examined = consumed = sequence.Start;
                }

                if (result.IsCompleted)
                {
                    break;
                }

                input.AdvanceTo(consumed, examined);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unexpected exception while reading from client");
                break;
            }
        }
    }

    private async Task WritingAsync()
    {
        var output = PipeWriter.Create(stream);

        await foreach (var packet in outgoing.Reader.ReadAllAsync().ConfigureAwait(false))
        {
            try
            {
                var serializer = packet.Create();
                var span = output.GetSpan(serializer.Length + sizeof(long));

                output.Advance(Protocol.Write(span, packet, serializer));
                await output.FlushAsync().ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Exception while writing");
                break;
            }
        }
    }

    private void Handshake(Packet packet)
    {
        var handshake = packet.Create<HandshakePacket>();

        state = handshake.State;

        if (state is State.Login or State.Play || handshake.Version is 47)
        {
            return;
        }

        var outdated = eventDispatcher.Dispatch(this, new Outdated());
        Write(state is State.Play ? new DisconnectPacket(outdated.Message) : new FailurePacket(outdated.Message));

        Stop();
    }

    private void Status(Packet packet)
    {
        if (packet.Identifier == StatusRequestPacket.Identifier)
        {
            var status = eventDispatcher.Dispatch(this, new Status());

            Write(new StatusResponsePacket(
                status.Name,
                status.Numerical,
                status.Maximum,
                status.Online,
                status.Description,
                status.Favicon));

            return;
        }

        Write(new PongPacket(packet.Create<PingPacket>().Magic));
        Stop();
    }

    private void Login(Packet packet)
    {
        var start = packet.Create<StartPacket>();
        var joining = eventDispatcher.Dispatch(this, new Login(start.Username));

        // Quit before switching to play state if token was cancelled.
        source.Token.ThrowIfCancellationRequested();

        if (joining.World is World world)
        {
            player = new Player(this, Guid.NewGuid().ToString(), start.Username, world);

            Write(
                new SuccessPacket(player.Unique, start.Username),
                new JoinGamePacket(
                    player.Identifier,
                    player.GameMode,
                    world.Dimension,
                    world.Difficulty,
                    byte.MaxValue,
                    world.Type, false),
                new PositionLookPacket(new Location(), 0, 0));

            state = State.Play;

            eventDispatcher.Dispatch(player, new Joined());
        }
        else
        {
            logger.LogWarning("No joining world specified.");
        }
    }

    private void Play(Packet packet)
    {
    }
}

internal enum State
{
    Handshake,
    Status,
    Login,
    Play
}