using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Packets;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Extensions;

public static class EntityExtensions
{
    extension(IWorld world)
    {
        /// <summary>
        /// Creates an entity and shows it to the players in the world.
        /// </summary>
        /// <param name="position">The entity's position.</param>
        /// <param name="configure">Changes the entity before it is shown.</param>
        public T Spawn<T>(Position position, Action<T>? configure = null) where T : ISpawnable
        {
            var entity = world.Create<T>(position);
            configure?.Invoke(entity);

            foreach (var pair in world.Players)
            {
                entity.Show(pair.Value);
            }

            return entity;
        }

        /// <summary>
        /// Strikes lightning for the players in the world.
        /// </summary>
        public void Strike(Position position)
        {
            // Clients only show lightning for a moment, so the world never keeps it.
            var lightning = world.Create<ILightning>(position);
            world.Remove(lightning);

            Broadcast(world, new SpawnGlobalEntityPacket(lightning));
        }
    }

    extension(ISpawnable entity)
    {
        /// <summary>
        /// Stops the world from tracking the entity and hides it from the players in the world.
        /// </summary>
        public void Despawn()
        {
            entity.World.Remove(entity);
            Broadcast(entity.World, new DestroyEntitiesPacket(entity));
        }

        /// <summary>
        /// Shows the entity as it currently is to a single player.
        /// </summary>
        public void Show(IPlayer player)
        {
            player.Client.Write(Spawning(entity));
        }

        /// <summary>
        /// Hides the entity from a single player, without the world forgetting it.
        /// </summary>
        public void Hide(IPlayer player)
        {
            player.Client.Write(new DestroyEntitiesPacket(entity));
        }

        /// <summary>
        /// Moves the entity and shows the players in the world where it went.
        /// </summary>
        public void Teleport(Position position)
        {
            entity.Position = position;
            Broadcast(entity.World, new EntityTeleportPacket(entity));
        }

        /// <summary>
        /// Rotates the entity and shows the players in the world where it looks.
        /// </summary>
        public void Look(float yaw, float pitch)
        {
            entity.Yaw = yaw;
            entity.Pitch = pitch;

            // Teleporting only rotates the body, the head has to be turned separately.
            Broadcast(entity.World, new EntityTeleportPacket(entity), new EntityHeadLookPacket(entity));
        }

        /// <summary>
        /// Sets the velocity and shows it to the players in the world, which then move the entity by it on their own.
        /// </summary>
        public void Push(Velocity velocity)
        {
            entity.Velocity = velocity;
            Broadcast(entity.World, new EntityVelocityPacket(entity, velocity));
        }

        /// <summary>
        /// Shows the players in the world the entity's current metadata and equipment.
        /// </summary>
        public void Update()
        {
            List<IOutgoingPacket> packets = [new EntityMetadataPacket(entity)];

            if (entity is ILiving living)
            {
                packets.AddRange(Equipment(living));
            }

            Broadcast(entity.World, [.. packets]);
        }
    }

    private static void Broadcast(IWorld world, params ReadOnlySpan<IOutgoingPacket> packets)
    {
        foreach (var pair in world.Players)
        {
            pair.Value.Client.Write(packets);
        }
    }

    private static IOutgoingPacket[] Spawning(ISpawnable entity)
    {
        List<IOutgoingPacket> packets = [];

        switch (entity)
        {
            // Mobs are spawned with their metadata and velocity.
            case IMob mob:
                packets.Add(new SpawnMobPacket(mob));
                break;

            case IPainting painting:
                packets.Add(new SpawnPaintingPacket(painting));
                break;

            case IExperienceOrb orb:
                packets.Add(new SpawnExperienceOrbPacket(orb));
                break;

            case ILightning lightning:
                packets.Add(new SpawnGlobalEntityPacket(lightning));
                break;

            // Objects are spawned without their metadata, and only some with their velocity.
            default:
                packets.Add(new SpawnObjectPacket(entity));
                packets.Add(new EntityMetadataPacket(entity));

                if (entity.Velocity is not { X: 0D, Y: 0D, Z: 0D })
                {
                    packets.Add(new EntityVelocityPacket(entity, entity.Velocity));
                }

                break;
        }

        if (entity is ILiving living)
        {
            // Clients spawn entities with nothing equipped.
            packets.AddRange(Equipment(living).Where(packet => !packet.Item.IsEmpty));
        }

        return [.. packets];
    }

    private static EntityEquipmentPacket[] Equipment(ILiving living)
    {
        return
        [
            new EntityEquipmentPacket(living, EquipmentSlot.Held, living.Held),
            new EntityEquipmentPacket(living, EquipmentSlot.Boots, living.Boots),
            new EntityEquipmentPacket(living, EquipmentSlot.Leggings, living.Leggings),
            new EntityEquipmentPacket(living, EquipmentSlot.Chestplate, living.Chestplate),
            new EntityEquipmentPacket(living, EquipmentSlot.Helmet, living.Helmet)
        ];
    }
}