using Amethyst.Api.Components;

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

    public void Write(ref MemoryWriter writer)
    {
        writer.WriteVariableInteger(Matches.Length);

        foreach (var match in Matches)
        {
            writer.WriteVariableString(match);
        }
    }

    public Task HandleAsync(Client client)
    {
        var text = Matches[0];

        if (!text.StartsWith('/'))
        {
            client.Transport.Queue(
                new TabCompletePacket
                {
                    Matches = client.Server.Players
                        .Select(player => player.Username)
                        .ToArray()
                });

            return Task.CompletedTask;
        }

        var commands = client.Server.CommandService.Commands.Keys.Select(match => $"/{match}");

        var matches = text.Length == 1
            ? commands
            : commands.Where(match => text[1] == match[1]);

        client.Transport.Queue(
            new TabCompletePacket
            {
                Matches = matches.ToArray()
            });

        return Task.CompletedTask;
    }
}