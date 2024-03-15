using Amethyst.Api.Entities;
using Amethyst.Api.World;
using Amethyst.Utilities;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class JoinGamePacket : IOutgoingPacket
{
    public int Identifier => 0x01;

    public required IPlayer Player { get; init; }

    public bool ReducedDebugInformation { get; init; }

    public int CalculateLength()
    {
        return sizeof(int)
               + sizeof(byte)
               + sizeof(sbyte)
               + sizeof(byte)
               + sizeof(byte)
               + VariableString.GetBytesCount(LevelType.Flat.ToString())
               + sizeof(bool);
    }

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteInteger(Player.Identifier);
        writer.WriteByte((byte) Player.GameMode);
        writer.WriteByte((byte) Dimension.OverWorld);
        writer.WriteByte((byte) Difficulty.Peaceful);
        writer.WriteByte((byte) Player.Server.Status.PlayerInformation.Max);
        writer.WriteVariableString(LevelType.Flat.ToString().ToLower());
        writer.WriteBoolean(ReducedDebugInformation);
    }
}