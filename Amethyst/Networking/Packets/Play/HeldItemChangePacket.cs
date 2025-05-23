﻿using Amethyst.Eventing;
using Amethyst.Eventing.Player;

namespace Amethyst.Networking.Packets.Play;

internal sealed class HeldItemChangePacket(short index) : IIngoingPacket<HeldItemChangePacket>, IProcessor
{
    public static int Identifier => 9;

    public static HeldItemChangePacket Create(ReadOnlySpan<byte> span)
    {
        var reader = new SpanReader(span);
        return new HeldItemChangePacket(reader.ReadShort());
    }

    public void Process(Client client, EventDispatcher eventDispatcher)
    {
        client.Player!.Inventory.Index = index;
        eventDispatcher.Dispatch(new Inventory(), client.Player);
    }
}