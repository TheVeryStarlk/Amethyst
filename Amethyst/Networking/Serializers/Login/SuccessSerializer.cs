using Amethyst.Abstractions.Networking;
using Amethyst.Abstractions.Networking.Packets.Login;

namespace Amethyst.Networking.Serializers.Login;

internal sealed class SuccessSerializer(int length) : ISerializer<SuccessPacket>
{
    public int Identifier => 2;

    public int Length => length;

    public static ISerializer Create(SuccessPacket packet)
    {
        return new SuccessSerializer(3);
    }

    public void Write(Span<byte> span)
    {
        SpanWriter.Create(span).WriteVariableString("Guid").WriteVariableString("Username");
    }
}