using Amethyst.Api.Components;
using Amethyst.Api.Events.Minecraft.Player;
using Amethyst.Api.Levels.Blocks;

namespace Amethyst.Networking.Packets.Playing;

internal sealed class PlayerBlockPlacementPacket : IIngoingPacket<PlayerBlockPlacementPacket>
{
    public static int Identifier => 0x08;

    public required Position Position { get; init; }

    public required BlockFace Face { get; init; }

    public required Item Item { get; init; }

    public required Position Cursor { get; init; }

    public static PlayerBlockPlacementPacket Read(MemoryReader reader)
    {
        var position = reader.ReadPosition();
        var face = (BlockFace) reader.ReadByte();

        var item = new Item
        {
            Type = reader.ReadShort()
        };

        if (item.Type != -1)
        {
            item.Amount = reader.ReadByte();

            // In case of blocks, durability stores the metadata.
            item.Durability = reader.ReadShort();

            if (reader.ReadByte() != 0)
            {
                // Read NBT.
            }
        }

        return new PlayerBlockPlacementPacket
        {
            Position = position,
            Face = face,
            Item = item,
            Cursor = new Position(reader.ReadByte(), reader.ReadByte(), reader.ReadByte())
        };
    }

    public async Task HandleAsync(Client client)
    {
        if (!client.Player!.Chunks.Any(chunk => chunk.X == Position.X >> 4 && chunk.Z == Position.Z >> 4))
        {
            return;
        }

        if (!Enum.TryParse(Face.ToString(), true, out BlockFace face)
            || !Enum.IsDefined(typeof(BlockFace), face))
        {
            return;
        }

        var position = Position;

        position = face switch
        {
            BlockFace.NegativeY => Position with
            {
                Y = position.Y - 1
            },
            BlockFace.PositiveY => Position with
            {
                Y = position.Y + 1
            },
            BlockFace.NegativeZ => Position with
            {
                Z = position.Z - 1
            },
            BlockFace.PositiveZ => Position with
            {
                Z = position.Z + 1
            },
            BlockFace.NegativeX => Position with
            {
                X = position.X - 1
            },
            BlockFace.PositiveX => Position with
            {
                X = position.X + 1
            },
            _ => throw new ArgumentException("Unknown face.")
        };

        var world = client.Server.Level?.Worlds.FirstOrDefault().Value;
        var block = new Block(Item.Type);

        var eventArgs = await client.Server.EventService.ExecuteAsync(
            new BlockPlaceEventArgs
            {
                Server = client.Server,
                Player = client.Player!,
                Block = block,
                Position = position,
                Sound = SoundEffect.DigWood
            });

        client.Server.BroadcastPacket(
            new SoundEffectPacket
            {
                Effect = eventArgs.Sound,
                Position = eventArgs.Position
            });

        client.Server.BroadcastPacket(
            new BlockChangePacket
            {
                Position = eventArgs.Position,
                Block = eventArgs.Block
            });

        world?.SetBlock(block, position);
    }
}