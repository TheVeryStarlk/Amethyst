using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Networking.Packets.Status;

public sealed class StatusResponsePacket(
    string name,
    int numerical,
    int maximum,
    int online,
    Message description,
    string favicon) : IOutgoingPacket
{
    public int Identifier => 0;

    public string Name => name;

    public int Numerical => numerical;

    public int Maximum => maximum;

    public int Online => online;

    public Message Description => description;

    public string Favicon => favicon;
}