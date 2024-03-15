using Amethyst.Api.Commands;
using Amethyst.Extensions;
using Amethyst.Utilities;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class TabCompletePacket : IIngoingPacket<TabCompletePacket>, IOutgoingPacket
{
    static int IIngoingPacket<TabCompletePacket>.Identifier => 0x14;

    public int Identifier => 0x3A;

    public required string[] Matches { get; init; }

    public Position? Position { get; init; }

    public static TabCompletePacket Read(MemoryReader reader)
    {
        return new TabCompletePacket
        {
            Matches =
            [
                reader.ReadVariableString()
            ],
            Position = reader.ReadBoolean() ? reader.ReadPosition() : null
        };
    }

    public int CalculateLength()
    {
        return VariableInteger.GetBytesCount(Matches.Length) + Matches.Sum(VariableString.GetBytesCount);
    }

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteVariableInteger(Matches.Length);

        foreach (var match in Matches)
        {
            writer.WriteVariableString(match);
        }
    }

    public async Task HandleAsync(Client client)
    {
        var text = Matches[0];

        if (text.Contains('/') && !text.Contains(' '))
        {
            await client.Transport.Output.WritePacketAsync(
                new TabCompletePacket
                {
                    Matches = client.Server.CommandService.Commands.Keys
                        .Select(match => $"/{match}")
                        .ToArray()
                });
        }
    }
}