namespace Amethyst.Abstractions.Networking.Packets.Login;

public sealed record LoginSuccessPacket(string Unique, string Username) : IOutgoingPacket;