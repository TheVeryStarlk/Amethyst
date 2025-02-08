using Amethyst.Components;
using Amethyst.Components.Eventing.Sources.Client;
using Amethyst.Components.Messages;
using Amethyst.Components.Protocol;
using Amethyst.Eventing;
using Amethyst.Protocol;
using Amethyst.Protocol.Packets.Handshake;
using Amethyst.Protocol.Packets.Login;
using Amethyst.Protocol.Packets.Play;
using Amethyst.Protocol.Packets.Status;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Client(ILogger<Client> logger, ConnectionContext connection, EventDispatcher eventDispatcher)
    : IClient, IAsyncDisposable
{
    public int Identifier { get; } = Random.Shared.Next();

    private readonly CancellationTokenSource source = CancellationTokenSource.CreateLinkedTokenSource(connection.ConnectionClosed);
    private readonly ProtocolWriter writer = new(connection.Transport.Output);
    private readonly SemaphoreSlim semaphore = new(1);

    private State state;
    private Message reason = "No reason specified.";

    public async Task StartAsync()
    {
        var reader = new ProtocolReader(connection.Transport.Input);

        while (true)
        {
            try
            {
                var packet = await reader.ReadAsync(source.Token).ConfigureAwait(false);

                var task = state switch
                {
                    State.Handshake => HandshakeAsync(packet),
                    State.Status => StatusAsync(packet),
                    State.Login => LoginAsync(packet),
                    State.Play => PlayAsync(packet),
                    _ => throw new ArgumentOutOfRangeException(nameof(state), state, "Invalid state.")
                };

                await task.ConfigureAwait(false);
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
            var request = await eventDispatcher.DispatchAsync(this, new Request(), source.Token).ConfigureAwait(false);
            await WriteAsync(new StatusResponsePacket(request.Status.Serialize())).ConfigureAwait(false);

            return;
        }

        await WriteAsync(packet.Create<PingPongPacket>()).ConfigureAwait(false);
        Stop("Finished ping.");
    }

    private async Task LoginAsync(Packet packet)
    {
        var loginStart = packet.Create<LoginStartPacket>();
        await WriteAsync(new LoginFailurePacket(reason.Serialize())).ConfigureAwait(false);
    }

    private Task PlayAsync(Packet packet)
    {
        return Task.CompletedTask;
    }
}

internal enum State
{
    Handshake,
    Status,
    Login,
    Play
}