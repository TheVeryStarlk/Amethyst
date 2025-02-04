using Amethyst.Components;
using Amethyst.Components.Messages;
using Amethyst.Entities;
using Amethyst.Eventing;
using Amethyst.Eventing.Sources.Clients;
using Amethyst.Eventing.Sources.Players;
using Amethyst.Eventing.Sources.Servers;
using Amethyst.Protocol;
using Amethyst.Protocol.Packets.Handshake;
using Amethyst.Protocol.Packets.Login;
using Amethyst.Protocol.Packets.Play;
using Amethyst.Protocol.Packets.Status;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Amethyst;

public sealed class Client(
    ILogger<Client> logger,
    ConnectionContext connection,
    EventDispatcher eventDispatcher,
    Server server) : IAsyncDisposable
{
    public int Identifier { get; } = Random.Shared.Next();

    private Player? player;

    private readonly CancellationTokenSource source = CancellationTokenSource.CreateLinkedTokenSource(connection.ConnectionClosed);
    private readonly ProtocolWriter writer = new(connection.Transport.Output);
    private readonly SemaphoreSlim semaphore = new(1);

    private State state;
    private Message reason = "No reason specified.";

    internal async Task StartAsync()
    {
        var reader = new ProtocolReader(connection.Transport.Input);

        while (true)
        {
            try
            {
                var packet = await reader.ReadAsync(source.Token).ConfigureAwait(false);

                Func<Packet, Task> task = state switch
                {
                    State.Handshake => HandshakeAsync,
                    State.Status => StatusAsync,
                    State.Login => LoginAsync,
                    State.Play => PlayAsync,
                    _ => throw new ArgumentOutOfRangeException(nameof(state), state, "Invalid state.")
                };

                await task(packet).ConfigureAwait(false);
            }
            catch (Exception exception) when (exception is OperationCanceledException or ConnectionResetException)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Unexpected exception while reading from client");
                break;
            }
            finally
            {
                reader.Advance();
            }
        }

        if (state is not State.Status)
        {
            IOutgoingPacket final = state is State.Login
                ? new LoginFailurePacket(reason.Serialize())
                : new DisconnectPacket(reason.Serialize());

            // Token is cancelled here so the final packet has to be manually sent out.
            // And wait a single tick to let the client realize the final packet.
            await writer.WriteAsync(final, CancellationToken.None).ConfigureAwait(false);
            await Task.Delay(50, CancellationToken.None).ConfigureAwait(false);

            if (state is State.Play)
            {
                await eventDispatcher.DispatchAsync(player!, new Left(), CancellationToken.None).ConfigureAwait(false);
            }
        }

        connection.Abort();
    }

    public async ValueTask WriteAsync(params IOutgoingPacket[] packets)
    {
        await semaphore.WaitAsync(source.Token).ConfigureAwait(false);

        try
        {
            foreach (var packet in packets)
            {
                await writer.WriteAsync(packet, source.Token).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // Nothing.
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unexpected exception while writing to client");
        }
        finally
        {
            semaphore.Release();
        }
    }

    public void Stop(Message message)
    {
        reason = message;
        source.Cancel();
    }

    public ValueTask DisposeAsync()
    {
        source.Dispose();
        semaphore.Dispose();
        return connection.DisposeAsync();
    }

    private async Task HandshakeAsync(Packet packet)
    {
        var handshake = packet.Create<HandshakePacket>();

        state = (State) handshake.State;

        if (state is not State.Login || handshake.Version is 47)
        {
            return;
        }

        var outdated = await eventDispatcher.DispatchAsync(this, new Outdated(handshake.Version), source.Token).ConfigureAwait(false);
        Stop(outdated.Message);
    }

    private async Task StatusAsync(Packet packet)
    {
        if (packet.Identifier == StatusRequestPacket.Identifier)
        {
            var request = await eventDispatcher.DispatchAsync(server, new StatusRequest(), source.Token).ConfigureAwait(false);
            await WriteAsync(new StatusResponsePacket(request.Status.Serialize())).ConfigureAwait(false);

            return;
        }

        await WriteAsync(packet.Create<PingPongPacket>()).ConfigureAwait(false);
    }

    private async Task LoginAsync(Packet packet)
    {
        var loginStart = packet.Create<LoginStartPacket>();

        await eventDispatcher.DispatchAsync(this, new Login(loginStart.Username), source.Token).ConfigureAwait(false);
        await WriteAsync(new LoginSuccessPacket(Guid.NewGuid().ToString(), loginStart.Username)).ConfigureAwait(false);

        state = State.Play;
        player = new Player(server, this, loginStart.Username);

        await eventDispatcher.DispatchAsync(player, new Joined(), source.Token).ConfigureAwait(false);

        await WriteAsync(
            new JoinGamePacket(Identifier, 0, 0, 0, 1, "default", false),
            new PlayerPositionAndLookPacket(0, 0, 0, 0, 0, false)).ConfigureAwait(false);
    }

    private async Task PlayAsync(Packet packet)
    {
        await eventDispatcher.DispatchAsync(player!, new Received(packet), source.Token).ConfigureAwait(false);
    }
}

internal enum State
{
    Handshake,
    Status,
    Login,
    Play
}