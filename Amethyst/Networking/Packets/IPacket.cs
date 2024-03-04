﻿namespace Amethyst.Networking.Packets;

internal interface IPacket
{
    public int Identifier { get; }
}

internal interface IIngoingPacket<out TSelf> : IPacket where TSelf : IIngoingPacket<TSelf>
{
    public static abstract TSelf Read(MemoryReader reader);
}

internal interface IOutgoingPacket : IPacket
{
    public int Write(ref MemoryWriter writer);
}