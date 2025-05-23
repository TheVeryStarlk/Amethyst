﻿using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class DisconnectSerializer(string message) : ISerializer<DisconnectPacket, DisconnectSerializer>
{
    public int Length => Variable.GetByteCount(message);

    public static DisconnectSerializer Create(DisconnectPacket packet)
    {
        return new DisconnectSerializer(packet.Message.Serialize());
    }

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString(message);
    }
}