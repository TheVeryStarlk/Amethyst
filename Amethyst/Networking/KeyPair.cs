using System.Security.Cryptography;

namespace Amethyst.Networking;

// Like the vanilla server, a single 1024 bit key pair is generated once and shared by every client.
internal static class KeyPair
{
    private static readonly RSA rsa = RSA.Create(1024);
    private static readonly Lock gate = new();

    /// <summary>
    /// The public key encoded as an X.509 subject public key info, as the client expects.
    /// </summary>
    public static byte[] Public { get; } = rsa.ExportSubjectPublicKeyInfo();

    public static byte[] Decrypt(byte[] data)
    {
        // RSA instances are not guaranteed to be thread-safe.
        lock (gate)
        {
            return rsa.Decrypt(data, RSAEncryptionPadding.Pkcs1);
        }
    }
}