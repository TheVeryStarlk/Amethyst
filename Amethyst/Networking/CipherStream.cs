using System.Buffers;

namespace Amethyst.Networking;

// Passes everything through as is until encryption is enabled.
internal sealed class CipherStream(Stream stream) : Stream
{
    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => true;

    public override long Length => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    private Cipher? decryptor;
    private Cipher? encryptor;

    /// <summary>
    /// Encrypts everything read and written afterward.
    /// Bytes that were already read are not decrypted, which is fine as the client sends nothing between the encryption response and logging in.
    /// </summary>
    public void Encrypt(byte[] secret)
    {
        decryptor = new Cipher(secret);
        encryptor = new Cipher(secret);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return Read(buffer.AsSpan(offset, count));
    }

    public override int Read(Span<byte> buffer)
    {
        var read = stream.Read(buffer);
        decryptor?.Decrypt(buffer[..read]);

        return read;
    }

    public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return ReadAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();
    }

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        var read = await stream.ReadAsync(buffer, cancellationToken).ConfigureAwait(false);
        decryptor?.Decrypt(buffer.Span[..read]);

        return read;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        Write(buffer.AsSpan(offset, count));
    }

    public override void Write(ReadOnlySpan<byte> buffer)
    {
        if (encryptor is null)
        {
            stream.Write(buffer);
            return;
        }

        var rented = ArrayPool<byte>.Shared.Rent(buffer.Length);

        try
        {
            var span = rented.AsSpan(0, buffer.Length);

            buffer.CopyTo(span);
            encryptor.Encrypt(span);

            stream.Write(span);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rented);
        }
    }

    public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
        return WriteAsync(buffer.AsMemory(offset, count), cancellationToken).AsTask();
    }

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (encryptor is null)
        {
            await stream.WriteAsync(buffer, cancellationToken).ConfigureAwait(false);
            return;
        }

        // The written buffer is read-only, so it is encrypted into a copy.
        var rented = ArrayPool<byte>.Shared.Rent(buffer.Length);

        try
        {
            var memory = rented.AsMemory(0, buffer.Length);

            buffer.CopyTo(memory);
            encryptor.Encrypt(memory.Span);

            await stream.WriteAsync(memory, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rented);
        }
    }

    public override void Flush()
    {
        stream.Flush();
    }

    public override Task FlushAsync(CancellationToken cancellationToken)
    {
        return stream.FlushAsync(cancellationToken);
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            decryptor?.Dispose();
            encryptor?.Dispose();
            stream.Dispose();
        }

        base.Dispose(disposing);
    }
}