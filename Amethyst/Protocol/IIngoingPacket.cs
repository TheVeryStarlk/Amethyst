using Amethyst.Api;
using Amethyst.Api.Entities;
using Amethyst.Protocol.Transport;

namespace Amethyst.Protocol;

internal interface IIngoingPacket<out TSelf> where TSelf : IIngoingPacket<TSelf>
{
    public static abstract int Identifier { get; }

    public static abstract TSelf Read(MemoryReader reader);

    public Task HandleAsync(IServer server, IPlayer player, IClient client)
    {
        return Task.CompletedTask;
    }
}

internal sealed record Message(int Identifier, ReadOnlyMemory<byte> Memory)
{
    public T As<T>() where T : IIngoingPacket<T>
    {
        if (T.Identifier != Identifier)
        {
            throw new InvalidOperationException($"Tried to read 0x{T.Identifier:X2} as 0x{Identifier:X2}.");
        }

        var reader = new MemoryReader(Memory);
        return T.Read(reader);
    }
}