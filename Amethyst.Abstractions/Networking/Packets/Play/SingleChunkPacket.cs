namespace Amethyst.Abstractions.Networking.Packets.Play;

public sealed record SingleChunkPacket(int X, int Z, byte[] Sections, ushort Bitmask) : IOutgoingPacket;