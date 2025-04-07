namespace Amethyst.Abstractions.Networking.Packets.Login;

internal sealed class SuccessPacket(string unique, string username) : IOutgoingPacket
{
    public int Identifier => 2;

    public string Unique => unique;

    public string Username => username;
}