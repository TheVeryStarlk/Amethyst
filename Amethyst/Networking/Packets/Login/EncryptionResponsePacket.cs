using System.Security.Cryptography;
using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Packets.Login;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Entities.Players;
using Amethyst.Worlds;
using Amethyst.Eventing;
using Amethyst.Abstractions.Eventing.Player;

namespace Amethyst.Networking.Packets.Login;

internal sealed class EncryptionResponsePacket(byte[] secret, byte[] token) : IIngoingPacket<EncryptionResponsePacket>, IProcessor
{
    public static int Identifier => 1;

    private readonly Message taken = Message.Simple("Username already taken!");

    public static EncryptionResponsePacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);

        return new EncryptionResponsePacket(
            reader.Read(reader.ReadVariableInteger()).ToArray(),
            reader.Read(reader.ReadVariableInteger()).ToArray());
    }

    public void Process(Client client, Dispatcher dispatcher)
    {
        // Both are encrypted with the public key, the shared secret is a 128 bit key.
        if (client.Verification is not { } verification
            || !CryptographicOperations.FixedTimeEquals(KeyPair.Decrypt(token), verification.Token)
            || KeyPair.Decrypt(secret) is not { Length: 16 } decrypted)
        {
            client.Stop();
            return;
        }

        client.Verification = null;
        client.Encrypt(decrypted);

        var joining = verification.Joining;

        var world = joining.World as World ?? throw new InvalidOperationException("No world was set.");
        var player = new Player(client, world, Guid.NewGuid(), joining.GameMode, joining.Username);

        // Usernames are only unique within a world ignoring case, unless UsernameSubscriber is registered.
        // The player is only assigned to the client once added, so a rejected player never dispatches a left event.
        if (!world.TryAdd(player))
        {
            client.Write(new FailurePacket(taken));
            client.Stop();

            return;
        }

        client.Player = player;

        client.Write(
            new SuccessPacket(player),
            new JoinGamePacket(player, world, byte.MaxValue, false),
            new PositionLookPacket(new Position(), 0, 0));

        client.State = State.Play;
        dispatcher.Dispatch(player, new Joined());
    }
}