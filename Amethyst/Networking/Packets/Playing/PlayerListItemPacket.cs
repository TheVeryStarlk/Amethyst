using Amethyst.Api.Components;
using Amethyst.Api.Entities;
using Amethyst.Extensions;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class PlayerListItemPacket : IOutgoingPacket
{
    public int Identifier => 0x38;

    public required PlayerListItemActionBase Action { get; init; }

    public required IEnumerable<IPlayer> Players { get; init; }

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

    public abstract void Write(IPlayer player, ref MemoryWriter writer);
}

internal sealed record AddPlayerAction : PlayerListItemActionBase
{
    public override int Identifier => 0;

    public override void Write(IPlayer player, ref MemoryWriter writer)
    {
        writer.WriteVariableString(player.Username);
        writer.WriteVariableInteger(0);
        writer.WriteVariableInteger((int) player.GameMode);
        writer.WriteVariableInteger(0);
        writer.WriteBoolean(true);
        writer.WriteVariableString(ChatMessage.Create(player.Username).Serialize());
    }
}

internal sealed record RemovePlayerAction : PlayerListItemActionBase
{
    public override int Identifier => 4;

    public override void Write(IPlayer player, ref MemoryWriter writer)
    {
    }
}