using Starlk.Networking;

namespace Amethyst.Protocol;

public sealed record Message(int Identifier, ReadOnlyMemory<byte> Memory)
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