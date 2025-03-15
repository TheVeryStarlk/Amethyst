using System.Buffers;

namespace Amethyst.Networking.Packets;

internal readonly record struct Packet(int Identifier, ReadOnlySequence<byte> Sequence);