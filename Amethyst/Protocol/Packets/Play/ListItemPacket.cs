using System.Globalization;
using System.Numerics;
using Amethyst.Components;
using Amethyst.Components.Entities;
using Amethyst.Components.Messages;
using Amethyst.Components.Protocol;

namespace Amethyst.Protocol.Packets.Play;

public sealed record ListItemPacket(ListItemAction Action, IPlayer Player) : IOutgoingPacket
{
    public int Identifier => 56;

    public int Length => Variable.GetByteCount(Action.Identifier)
                         + Variable.GetByteCount(1)
                         + Action.Length(Player);

    public void Write(Span<byte> span)
    {
        var writer = SpanWriter
            .Create(span)
            .WriteVariableInteger(Action.Identifier)
            .WriteVariableInteger(1);

        writer.Write(BigInteger.Parse(Player.Guid.ToString().Replace("-", ""), NumberStyles.HexNumber).ToByteArray(isBigEndian: true));
        Action.Write(Player, span[(Variable.GetByteCount(Action.Identifier) + Variable.GetByteCount(1) + 16)..]);
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

    public override int Length(IPlayer player)
    {
        return Variable.GetByteCount(player.Username)
               + Variable.GetByteCount(0)
               + Variable.GetByteCount(1)
               + Variable.GetByteCount(0)
               + sizeof(bool)
               + Variable.GetByteCount(Message.Create(player.Username).Serialize())
               + 16;
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
            .WriteVariableString(Message.Create(player.Username).Serialize());
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