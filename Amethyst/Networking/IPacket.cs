namespace Amethyst.Networking;

internal interface IPacket;

internal interface IIngoingPacket<out TSelf> : IPacket where TSelf : IIngoingPacket<TSelf>
{
    public static abstract int Identifier { get; }

    public static abstract TSelf Read(MemoryReader reader);

    public Task HandleAsync(Client client)
    {
        return Task.CompletedTask;
    }
}

internal interface IOutgoingPacket : IPacket
{
    public int Identifier { get; }

    public int CalculateLength();

    public int Write(ref MemoryWriter writer);
}