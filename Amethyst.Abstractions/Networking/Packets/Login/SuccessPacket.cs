namespace Amethyst.Abstractions.Networking.Packets.Login;

public sealed record LoginSuccessPacket(Guid Guid, string Username) : IOutgoingPacket;