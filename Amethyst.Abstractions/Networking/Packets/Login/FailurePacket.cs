using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Networking.Packets.Login;

public sealed record FailurePacket(Message Message) : IOutgoingPacket;