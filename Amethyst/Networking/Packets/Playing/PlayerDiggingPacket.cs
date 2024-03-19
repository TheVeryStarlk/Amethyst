using Amethyst.Api.Components;
using Amethyst.Api.Entities;
using Amethyst.Api.Events.Minecraft.Player;
using Amethyst.Api.Levels.Blocks;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class PlayerDiggingPacket : IIngoingPacket<PlayerDiggingPacket>
{
    public static int Identifier => 0x07;

    public required DiggingStatus Status { get; init; }

    public required Position Position { get; init; }

    public required BlockFace Face { get; init; }

    public static PlayerDiggingPacket Read(MemoryReader reader)
    {
        return new PlayerDiggingPacket
        {
            Status = (DiggingStatus) reader.ReadByte(),
            Position = reader.ReadPosition(),
            Face = (BlockFace) reader.ReadByte()
        };
    }

    public async Task HandleAsync(Client client)
    {
        var world = client.Server.Level?.Worlds.FirstOrDefault().Value;

        if (Status is DiggingStatus.StartedDigging || client.Player!.GameMode is GameMode.Creative)
        {
            var block = new Block(0);

            var eventArgs = await client.Server.EventService.ExecuteAsync(
                new BlockBreakEventArgs
                {
                    Server = client.Server,
                    Player = client.Player!,
                    Block = block,
                    Position = Position
                });

            client.Server.BroadcastPacket(
                new BlockChangePacket
                {
                    Position = eventArgs.Position,
                    Block = eventArgs.Block
                });

            world?.SetBlock(block, Position);
        }
    }
}

internal enum DiggingStatus
{
    StartedDigging,
    CancelledDigging,
    FinishedDigging,
    DropItemStack,
    DropItem,
    ShootArrowOrFinishEating
}