using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Packets.Play;

namespace Amethyst.Networking.Serializers.Play;

internal sealed class ListItemSerializer(IListItemAction action, IPlayer player) : ISerializer<ListItemPacket, ListItemSerializer>
{
    public int Length
    {
        get
        {
            var length = Variable.GetByteCount(action.Identifier) + Variable.GetByteCount(1) + 16;

            length += action switch
            {
                AddPlayerAction current => Variable.GetByteCount(current.Username)
                                           + Variable.GetByteCount(0)
                                           + Variable.GetByteCount((int) current.GameMode)
                                           + Variable.GetByteCount(current.Latency)
                                           + sizeof(bool)
                                           + Variable.GetByteCount(current.Tag),

                RemovePlayerAction => 0,
                _ => throw new ArgumentOutOfRangeException(nameof(action))
            };

            return length;
        }
    }

    public static ListItemSerializer Create(ListItemPacket packet)
    {
        return new ListItemSerializer(packet.Action, packet.Player);
    }

    public void Write(Span<byte> span)
    {
        var writer = SpanWriter
            .Create(span)
            .WriteVariableInteger(action.Identifier)
            .WriteVariableInteger(1)
            .WriteGuid(player.Guid);

        switch (action)
        {
            case AddPlayerAction current:
                writer
                    .WriteVariableString(current.Username)
                    .WriteVariableInteger(0)
                    .WriteVariableInteger((int) current.GameMode)
                    .WriteVariableInteger(current.Latency)
                    .WriteBoolean(true)
                    .WriteVariableString(current.Tag);

                break;

            case RemovePlayerAction current:
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(action));
        }
    }
}