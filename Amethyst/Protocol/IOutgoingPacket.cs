using Starlk.Networking;

namespace Amethyst.Protocol;

public interface IOutgoingPacket
{
    public int Identifier { get; }

    public int CalculateLength()
    {
        // Most packets are way smaller than this.
        // Although some like the chunk packets are bigger.
        return short.MaxValue;
    }

    public void Write(ref MemoryWriter writer);
}