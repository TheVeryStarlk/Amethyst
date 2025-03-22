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
    public EventDispatcher EventDispatcher => eventDispatcher;

    public Player? Player { get; private set; }

    private readonly NetworkStream stream = new(socket);
    private readonly CancellationTokenSource source = new();
    private readonly Channel<IOutgoingPacket> outgoing = Channel.CreateUnbounded<IOutgoingPacket>();

    private State state;

    public async Task StartAsync()
    {
        var reading = ReadingAsync();
        var writing = WritingAsync();

        if (await Task.WhenAny(reading, writing).ConfigureAwait(false) == reading)
        {
            outgoing.Writer.Complete();
            await writing.ConfigureAwait(false);
        }

        source.Cancel();

        if (state is State.Play)
        {
            eventDispatcher.Dispatch(Player!, new Left());
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
        var login = eventDispatcher.Dispatch(this, new Login(start.Username));

        // Quit before switching to play state if token was cancelled.
        source.Token.ThrowIfCancellationRequested();

        if (login.World is not World world)
        {
            logger.LogWarning("No joining world specified.");
            return;
        }

        Player = new Player(this, Guid.NewGuid().ToString(), start.Username, world);

        Write(
            new SuccessPacket(Player.Unique, start.Username),
            new JoinGamePacket(
                Player.Identifier,
                Player.GameMode,
                world.Dimension,
                world.Difficulty,
                byte.MaxValue,
                world.Type,
                false),
            new PositionLookPacket(new Location(), 0, 0));

        state = State.Play;

        eventDispatcher.Dispatch(Player, new Joined());
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