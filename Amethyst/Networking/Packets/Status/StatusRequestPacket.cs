using Amethyst.Api.Components;
using Amethyst.Api.Events.Minecraft;
using Amethyst.Extensions;

namespace Amethyst.Networking.Packets.Status;

internal sealed class StatusRequestPacket: IIngoingPacket<StatusRequestPacket>
{
    public static int Identifier => 0x00;

    public static StatusRequestPacket Read(MemoryReader reader)
    {
        return new StatusRequestPacket();
    }

    public async Task HandleAsync(Client client)
    {
        var eventArgs = await client.Server.EventService.ExecuteAsync(
            new DescriptionRequestedEventArgs
            {
                Server = client.Server,
                Description = client.Server.Description
            });

        client.Server.Description = eventArgs.Description;

        await client.Transport.Output.WritePacketAsync(
            new StatusResponsePacket
            {
                Status = ServerStatus.Create(
                    nameof(Amethyst),
                    Server.ProtocolVersion,
                    client.Server.Configuration.MaximumPlayerCount,
                    client.Server.Players.Count(),
                    client.Server.Description)
            });
    }
}