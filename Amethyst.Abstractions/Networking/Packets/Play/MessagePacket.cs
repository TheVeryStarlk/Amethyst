using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed record MessagePacket(Message Message, MessagePosition Position) : IOutgoingPacket;