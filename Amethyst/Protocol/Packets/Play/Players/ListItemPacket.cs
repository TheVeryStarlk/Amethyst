using Amethyst.Components;
using Amethyst.Components.Entities;
using Amethyst.Components.Messages;
using Amethyst.Components.Protocol;

namespace Amethyst.Protocol.Packets.Play.Players;

// This currently only works with a single player.
public sealed record ListItemPacket(ListItemAction Action, params IPlayer[] Players) : IOutgoingPacket
{
    public int Identifier => 56;

    public int Length => Variable.GetByteCount(Action.Identifier)
                         + Variable.GetByteCount(Players.Length)
                         + Players.Sum(Action.Length)
                         + sizeof(long)
                         + sizeof(long);

    public void Write(Span<byte> span)
    {
        var writer = SpanWriter
            .Create(span)
            .WriteVariableInteger(Action.Identifier)
            .WriteVariableInteger(Players.Length);

        foreach (var player in Players)
        {
            writer.Write(player.Guid.ToArray());
            Action.Write(player, span[(Variable.GetByteCount(Action.Identifier) + Variable.GetByteCount(Players.Length) + sizeof(long) + sizeof(long))..]);
        }
    }
}

public abstract class ListItemAction
{
    public abstract int Identifier { get; }

    public abstract int Length(IPlayer player);

    public abstract void Write(IPlayer player, Span<byte> span);
}

public sealed class AddPlayerAction : ListItemAction
{
    public override int Identifier => 0;

    private string? username;

    public override int Length(IPlayer player)
    {
        username = Message.Create(player.Username).Serialize();

        return Variable.GetByteCount(player.Username)
               + Variable.GetByteCount(0)
               + Variable.GetByteCount(1)
               + Variable.GetByteCount(0)
               + sizeof(bool)
               + Variable.GetByteCount(username);
    }

    public override void Write(IPlayer player, Span<byte> span)
    {
        SpanWriter
            .Create(span)
            .WriteVariableString(player.Username)
            .WriteVariableInteger(0)
            .WriteVariableInteger(1)
            .WriteVariableInteger(0)
            .WriteBoolean(true)
            .WriteVariableString(username!);
    }
}

public sealed class RemovePlayerAction : ListItemAction
{
    public override int Identifier => 4;

    public override int Length(IPlayer player)
    {
        return 0;
    }

    public override void Write(IPlayer player, Span<byte> span)
    {
    }
}