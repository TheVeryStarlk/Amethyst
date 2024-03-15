using Amethyst.Api.Components;
using Amethyst.Api.Entities;
using Amethyst.Extensions;
using Amethyst.Utilities;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class PlayerListItemPacket : IOutgoingPacket
{
    public int Identifier => 0x38;

    public required PlayerListItemActionBase Action { get; init; }

    public required IEnumerable<IPlayer> Players { get; init; }

    public int CalculateLength()
    {
        return VariableInteger.GetBytesCount(Action.Identifier)
               + VariableInteger.GetBytesCount(Players.Count())
               + Players.Count() * 16
               + Players.Sum(player => Action.CalculateLength(player));
    }

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteVariableInteger(Action.Identifier);
        writer.WriteVariableInteger(Players.Count());

        foreach (var player in Players)
        {
            writer.WriteGuid(player.Guid);
            Action.Write(player, ref writer);
        }
    }
}

internal abstract record PlayerListItemActionBase
{
    public abstract int Identifier { get; }

    public abstract int CalculateLength(IPlayer player);

    public abstract void Write(IPlayer player, ref MemoryWriter writer);
}

internal sealed record AddPlayerAction : PlayerListItemActionBase
{
    public override int Identifier => 0;

    public override int CalculateLength(IPlayer player)
    {
        return VariableString.GetBytesCount(player.Username)
               + VariableInteger.GetBytesCount(0)
               + VariableInteger.GetBytesCount((int) player.GameMode)
               + VariableInteger.GetBytesCount(0)
               + sizeof(bool)
               + VariableString.GetBytesCount(ChatMessage.Create(player.Username).Serialize());
    }

    public override void Write(IPlayer player, ref MemoryWriter writer)
    {
        writer.WriteVariableString(player.Username);
        writer.WriteVariableInteger(0);
        writer.WriteVariableInteger((int) player.GameMode);
        writer.WriteVariableInteger(0);
        writer.WriteBoolean(true);
        writer.WriteVariableString(ChatMessage.Create(player.Username).Serialize()!);
    }
}

internal sealed record RemovePlayerAction : PlayerListItemActionBase
{
    public override int Identifier => 4;

    public override int CalculateLength(IPlayer player)
    {
        return 0;
    }

    public override void Write(IPlayer player, ref MemoryWriter writer)
    {
    }
}