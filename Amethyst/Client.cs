using Amethyst.Components;
using Amethyst.Components.Eventing.Sources.Client;
using Amethyst.Components.Messages;
using Amethyst.Eventing;
using Amethyst.Protocol;
using Amethyst.Protocol.Packets.Handshake;
using Amethyst.Protocol.Packets.Login;
using Amethyst.Protocol.Packets.Play;
using Amethyst.Protocol.Packets.Status;
using Microsoft.AspNetCore.Connections;
using Microsoft.Extensions.Logging;

namespace Amethyst;

internal sealed class Client(
    ILogger<Client> logger,
    ConnectionContext connection,
    EventDispatcher eventDispatcher,
    int identifier) : IClient, IAsyncDisposable
{
    public int Identifier => identifier;

    private readonly CancellationTokenSource source = CancellationTokenSource.CreateLinkedTokenSource(connection.ConnectionClosed);
    private readonly ProtocolDuplex protocol = connection.CreateProtocol();
    private readonly SemaphoreSlim semaphore = new(1);

    private State state;
    private Message message = "No reason specified.";

    public async Task StartAsync()
    {
        await ReadingAsync().ConfigureAwait(false);

        if (state is not State.Status)
        {
            IOutgoingPacket final = state is State.Login
                ? new LoginFailurePacket(message.Serialize())
                : new DisconnectPacket(message.Serialize());

            // Token is cancelled here so the final packet has to be manually sent out.
            // And wait a single tick to let the client realize the final packet.
            await protocol.Output.WriteAsync(final, CancellationToken.None).ConfigureAwait(false);
            await Task.Delay(50).ConfigureAwait(false);
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
                await protocol.Output.WriteAsync(packet, source.Token).ConfigureAwait(false);
            }
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

    public void Stop(Message reason)
    {
        message = reason;
        source.Cancel();
    }

    public ValueTask DisposeAsync()
    {
        source.Dispose();
        semaphore.Dispose();
        return connection.DisposeAsync();
    }

    private async Task ReadingAsync()
    {
        while (true)
        {
            try
            {
                var packet = await protocol.Input.ReadAsync(source.Token).ConfigureAwait(false);

                switch (state)
                {
                    case State.Handshake:
                        packet.Out(out HandshakePacket handshake);

                        state = (State) handshake.State;

                        if (state is State.Login && handshake.Version != 47)
                        {
                            // Eh, should probably cache this.
                            var reason = Message
                                .Create()
                                .Write("I'm still rocking 1.8!").Red()
                                .Build();

                            Stop(reason);
                        }

                        break;

                    case State.Status:
                        switch (packet.Identifier)
                        {
                            case 0:
                                packet.Out(out StatusRequestPacket _);

                                var request = await eventDispatcher.DispatchAsync(this, new StatusRequest(), source.Token).ConfigureAwait(false);
                                await WriteAsync(new StatusResponsePacket(request.Status.Serialize())).ConfigureAwait(false);

                                break;

                            case 1:
                                packet.Out(out PingPacket ping);
                                await WriteAsync(new PongPacket(ping.Magic)).ConfigureAwait(false);

                                break;
                        }

                        break;

                    case State.Login:
                        packet.Out(out LoginStartPacket loginStart);

                        var joining = await eventDispatcher.DispatchAsync(this, new Joining(loginStart.Username), source.Token).ConfigureAwait(false);

                        await WriteAsync(
                            new LoginSuccessPacket(Guid.NewGuid().ToString(), loginStart.Username),
                            new JoinGamePacket(Identifier, joining.GameMode, 0, 0, joining.MaximumPlayerCount, "default", joining.ReducedDebugInformation),
                            new PlayerPositionAndLookPacket(joining.X, joining.Y, joining.Z, joining.Yaw, joining.Pitch, false)).ConfigureAwait(false);

                        state = State.Play;

                        break;

                    case State.Play:
                        await eventDispatcher.DispatchAsync(this, new ReceivedPacket(packet), source.Token).ConfigureAwait(false);
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }
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
                protocol.Input.Advance();
            }
        }
    }
}

public enum State
{
    Handshake,
    Status,
    Login,
    Play
}