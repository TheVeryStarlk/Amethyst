<p align="center">
  <img width="100" height="100" align="center" src="https://i.imgur.com/BkTfea4.png">
</p>

# Amethyst

A light-weight library targeting a [specific protocol version](https://minecraft.wiki/w/Java_Edition_1.8.9) for developing Minecraft servers.
Amethyst is customizable and sacrifices many of the vanilla features in favor of performance and memory usage.

## Getting Started

Implementing [ISubscriber](https://github.com/TheVeryStarlk/Amethyst/blob/rewrite/Amethyst.Abstractions/Eventing/ISubscriber.cs) and registering Amethyst by `services.AddAmethyst<FooSubscriber>();` is all you need to get a running Amethyst server.
However, you need to create a world for players to join to.

```cs
// Create the world once, players are tracked by the world they are in.
var world = worldFactory.Create("Empty", EmptyGenerator.Instance);

registry.For<IClient>(consumer => consumer.On<Joining>((_, joining) =>
{
    joining.World = world;
}));
```

## Usage

Amethyst by default has very little built-in logic, the way you implement logic is by subscribing to events.
The following example sends a welcome message when a player joins.

```csharp
registry.For<IPlayer>(consumer => consumer.On<Joined>((player, _) =>
{
    var message = Message.Simple($"Welcome {player.Username}!");
    player.Send(message);
}));
```

Subscribers run in the order they were registered. Some events implement `ICancellable`, cancelling them skips the remaining subscribers and prevents the action.
The following example stops players from breaking blocks below `Y = 64`.

```csharp
registry.For<IPlayer>(consumer => consumer.On<Dig>((_, dig) =>
{
    dig.IsCancelled = dig.Position.Y < 64;
}));
```

Amethyst has no built-in tick loop, if you need one, run your own (for example, a hosted service with a `PeriodicTimer`).

### Subscribers

Amethyst's core only handles what the protocol requires. Gameplay behavior is opt-in through the subscribers in `Amethyst.Extensions`.
Without `ChunkSubscriber`, players receive no chunks. Without `BlockSubscriber`, blocks players break or place only change on their own client.

```csharp
services
    .AddAmethyst<FooSubscriber>()
    .AddSubscriber<UsernameSubscriber>() // Keeps usernames unique across worlds, ignoring case.
    .AddSubscriber<ChunkSubscriber>() // Streams chunks around players.
    .AddSubscriber<ListSubscriber>() // Keeps the tab list in sync.
    .AddSubscriber<VisibilitySubscriber>() // Spawns and moves players for each other.
    .AddSubscriber<BlockSubscriber>() // Applies broken and placed blocks to the world.
    .AddSubscriber<EntitySubscriber>(); // Shows the entities of a world to players joining it.
```

### Entities

Every 1.8 mob and object can be spawned, from zombies and horses to armor stands, minecarts and paintings. Entities are created by their interface, and their metadata is set through properties.
Amethyst runs no AI or physics, entities only move when told to. Changing an entity does not show the change to players until it is sent to them.

```csharp
var zombie = world.Spawn<IZombie>(new Position(0, 64, 0), zombie =>
{
    zombie.CustomName = "Bob";
    zombie.Helmet = new Item((short) ItemType.DiamondHelmet, 1, 0);
});

zombie.IsChild = true;
zombie.Update(); // Sends the changed metadata and equipment.

zombie.Teleport(new Position(0, 70, 0));
zombie.Despawn();
```

## Example

`Amethyst.Example` is a small creative server built on top of Amethyst. It generates an endless overworld with oceans, rivers, beaches, plains, forests, deserts, savannas, taigas and snowy mountains, decorated with ores, flowers and trees.
Players join with a hotbar of wands that grow trees, blast craters, blink forward and cover meadows in flowers, and can use `/help` to find their way around.

```
dotnet run --project Amethyst.Example -- --seed 1234 --port 25565
```

Both arguments are optional, then connect with a 1.8.9 client.

## Credits

* [Minecraft Wiki](https://minecraft.wiki/w/Protocol?oldid=2772100) for the amazing documentations about the game's internal technical details.
* [Obsidian](https://github.com/ObsidianMC/Obsidian) for being an awesome reference.