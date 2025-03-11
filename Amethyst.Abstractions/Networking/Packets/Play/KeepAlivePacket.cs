namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed record SingleChunkPacket(long Random) : IOutgoingPacket;