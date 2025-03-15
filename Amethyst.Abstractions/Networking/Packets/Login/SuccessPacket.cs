namespace Amethyst.Abstractions.Networking.Packets.Login;

public sealed record SuccessPacket(string Unique, string Username) : IOutgoingPacket;