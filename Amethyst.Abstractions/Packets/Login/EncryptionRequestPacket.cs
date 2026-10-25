namespace Amethyst.Abstractions.Packets.Login;

internal sealed class EncryptionRequestPacket(byte[] key, byte[] token) : IOutgoingPacket
{
    public int Identifier => 1;

    public byte[] Key => key;

    public byte[] Token => token;
}