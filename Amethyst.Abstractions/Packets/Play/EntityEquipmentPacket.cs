using Amethyst.Abstractions.Entities;

namespace Amethyst.Abstractions.Packets.Play;

public sealed class EntityEquipmentPacket(IEntity entity, EquipmentSlot slot, Item item) : IOutgoingPacket
{
    public int Identifier => 4;

    public IEntity Entity => entity;

    public EquipmentSlot Slot => slot;

    public Item Item => item;
}

public enum EquipmentSlot
{
    Held,
    Boots,
    Leggings,
    Chestplate,
    Helmet
}