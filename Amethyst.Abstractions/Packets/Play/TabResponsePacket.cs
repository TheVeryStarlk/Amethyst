﻿namespace Amethyst.Abstractions.Packets.Play;

public sealed class TabResponsePacket(string[] matches) : IOutgoingPacket
{
    public int Identifier => 58;

    public string[] Matches => matches;
}