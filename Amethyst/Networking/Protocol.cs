using Amethyst.Networking.Packets;

namespace Amethyst.Networking;

internal static class Protocol
{
    public static async Task<Message> ReadAsync(CancellationToken token)
    {
        await Task.CompletedTask;
        return new Message(0, Memory<byte>.Empty);
    }

    public static Task WriteAsync(IOutgoingPacket packet, CancellationToken token)
    {
        throw new NotImplementedException();
    }
}

internal sealed record Message(int Identifier, Memory<byte> Memory);