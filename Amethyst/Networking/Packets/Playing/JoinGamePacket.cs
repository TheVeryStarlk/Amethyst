using Amethyst.Api.Entities;
using Amethyst.Api.Levels;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class JoinGamePacket : IOutgoingPacket
{
    public int Identifier => 0x01;

    public required IPlayer Player { get; init; }

    public bool ReducedDebugInformation { get; init; }

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteInteger(Player.Identifier);
        writer.WriteByte((byte) Player.GameMode);
        writer.WriteByte((byte) Dimension.OverWorld);
        writer.WriteByte((byte) Difficulty.Peaceful);
        writer.WriteByte((byte) Player.Server.Configuration.MaximumPlayerCount);
        writer.WriteVariableString(WorldType.Flat.ToString().ToLower());
        writer.WriteBoolean(ReducedDebugInformation);
    }
}