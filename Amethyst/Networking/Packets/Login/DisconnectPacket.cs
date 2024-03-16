using Amethyst.Api.Components;
using Amethyst.Extensions;

namespace Amethyst.Networking.Packets.Login;

internal sealed class DisconnectPacket(ClientState state) : IOutgoingPacket
{
    public int Identifier => state is ClientState.Login ? 0x00 : 0x40;

    public required ChatMessage Reason { get; init; }

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteVariableString(Reason.Serialize());
    }
}