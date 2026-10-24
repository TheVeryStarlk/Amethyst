using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Abstractions.Worlds;
using Raspite.Tags.Building;

namespace Amethyst.Example;

/// <summary>
/// The explorer's starting hotbar: four wands and a handful of building blocks.
/// </summary>
internal static class Kit
{
    public static Item SaplingWand { get; } = Named(ItemType.Stick, 0, "Sapling Wand", "Right-click a block to grow", "a tree that suits the biome.");

    public static Item CraterRod { get; } = Named(ItemType.BlazeRod, 0, "Crater Rod", "Right-click a block to blast", "a smouldering crater.");

    public static Item BlinkFeather { get; } = Named(ItemType.Feather, 0, "Blink Feather", "Right-click to blink forward", "until something is in the way.");

    // Bone meal is the white dye.
    public static Item BloomDust { get; } = Named(ItemType.InkSack, 15, "Bloom Dust", "Right-click grass to cover", "the meadow around it in flowers.");

    private static readonly Item[] hotbar =
    [
        SaplingWand,
        CraterRod,
        BlinkFeather,
        BloomDust,
        Stack(Blocks.StainedClay, 10),
        Stack(Blocks.StainedGlass, 10),
        Stack(Blocks.StoneBricks),
        Stack(Blocks.Glowstone),
        Stack(Blocks.Wood)
    ];

    public static void Give(IPlayer player)
    {
        // The hotbar takes the last nine slots of the player's inventory window.
        for (var index = 0; index < hotbar.Length; index++)
        {
            player.Client.Write(new SetSlotPacket((short) (36 + index), hotbar[index]));
        }

        player.Client.Write(new HeldSlotPacket(0));
    }

    private static Item Stack(Block block, int metadata = 0)
    {
        return new Item((short) block.Type, 64, (short) metadata);
    }

    private static Item Named(ItemType type, short durability, string name, params string[] lore)
    {
        var display = CompoundTagBuilder
            .Create("display")
            .AddString($"§d§l{name}", "Name")
            .AddStringList(lore.Select(line => $"§7{line}").ToArray(), "Lore")
            .Build();

        var tag = CompoundTagBuilder
            .Create(string.Empty)
            .AddCompound(display, "display")
            .Build();

        return new Item((short) type, 1, durability, tag);
    }
}
