using System.Security.Cryptography;

namespace Amethyst.Networking;

// AES in CFB8 mode with the shared secret as both the key and the initial vector.
// Every ciphertext byte is shifted into the vector, so after each call the vector is the last 16 bytes of ciphertext,
// which lets the one-shot methods continue the stream across calls.
internal sealed class Cipher : IDisposable
{
    private readonly Aes aes = Aes.Create();
    private readonly byte[] vector = new byte[16];

    public Cipher(byte[] secret)
    {
        aes.Key = secret;
        secret.CopyTo(vector, 0);
    }

    public void Encrypt(Span<byte> span)
    {
        aes.EncryptCfb(span, vector, span, PaddingMode.None, 8);
        Shift(span, vector);
    }

    public void Decrypt(Span<byte> span)
    {
        // Decrypting in place overwrites the ciphertext, so the next vector is taken before.
        Span<byte> next = stackalloc byte[16];
        Shift(span, next);

        aes.DecryptCfb(span, vector, span, PaddingMode.None, 8);
        next.CopyTo(vector);
    }

    public void Dispose()
    {
        aes.Dispose();
    }

    private void Shift(ReadOnlySpan<byte> ciphertext, Span<byte> destination)
    {
        if (ciphertext.Length >= vector.Length)
        {
            ciphertext[^vector.Length..].CopyTo(destination);
            return;
        }

        vector.AsSpan(ciphertext.Length).CopyTo(destination);
        ciphertext.CopyTo(destination[^ciphertext.Length..]);
    }
}