using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed partial class EffectSerializer(int effect, Position position, int metadata, bool isGlobal) : ISerializer<EffectPacket, EffectSerializer>
{
    public static EffectSerializer Create(EffectPacket packet)
    {
        return new EffectSerializer(packet.Effect, packet.Position, packet.Metadata, packet.IsGlobal);
    }
}