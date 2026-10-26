using Amethyst.Abstractions.Packets.Login;

namespace Amethyst.Networking.Serializers.Login;

internal sealed class EncryptionRequestSerializer(byte[] key, byte[] token) : ISerializer<EncryptionRequestPacket, EncryptionRequestSerializer>
{
    // The server identifier is always empty.
    public int Length => Variable.GetByteCount(string.Empty)
                         + Variable.GetByteCount(key.Length) + key.Length
                         + Variable.GetByteCount(token.Length) + token.Length;

    public static EncryptionRequestSerializer Create(EncryptionRequestPacket packet)
    {
        return new EncryptionRequestSerializer(packet.Key, packet.Token);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteVariableString(string.Empty)
            .WriteVariableInteger(key.Length)
            .Write(key)
            .WriteVariableInteger(token.Length)
            .Write(token);
    }
}