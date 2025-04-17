namespace Amethyst.Abstractions.Packets.Login;

internal sealed class SuccessPacket(string guid, string username) : IOutgoingPacket
{
    public int Identifier => 2;

    public string Guid => guid;

    public string Username => username;
}