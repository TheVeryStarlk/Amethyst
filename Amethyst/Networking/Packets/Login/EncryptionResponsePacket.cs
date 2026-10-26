using System.Security.Cryptography;
using Amethyst.Eventing;

namespace Amethyst.Networking.Packets.Login;

internal sealed class EncryptionResponsePacket(byte[] secret, byte[] token) : IIngoingPacket<EncryptionResponsePacket>, IProcessor
{
    public static int Identifier => 1;

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

        StartPacket.Join(client, dispatcher, verification.Joining);
    }
}