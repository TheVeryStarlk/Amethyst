using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions.Packets.Play;

public sealed class SoundPacket(string name, Position position, float volume, float pitch) : IOutgoingPacket
{
    public int Identifier => 41;

    public string Name => name;

    public Position Position => position;

    public float Volume => volume;

    public float Pitch => pitch;
}