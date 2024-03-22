using Amethyst.Api.Entities;

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
        writer.WriteByte((byte) Player.World!.Dimension);
        writer.WriteByte((byte) Player.World.Difficulty);
        writer.WriteByte((byte) Player.Server.Configuration.MaximumPlayerCount);
        writer.WriteVariableString(Player.World.Type.ToString().ToLower());
        writer.WriteBoolean(ReducedDebugInformation);
    }
}