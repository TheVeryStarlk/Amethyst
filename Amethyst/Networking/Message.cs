namespace Amethyst.Networking;

internal sealed record Message(int Identifier, Memory<byte> Memory)
{
    public T As<T>() where T : IIngoingPacket<T>
    {
        if (T.Identifier != Identifier)
        {
            throw new ArgumentException($"Expected {T.Identifier} but got {Identifier} instead.");
        }

        return T.Read(new MemoryReader(Memory));
    }
}