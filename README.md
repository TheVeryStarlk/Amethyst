<p align="center">
  <img width="100" height="100" align="center" src="https://i.imgur.com/BkTfea4.png">
</p>

# Amethyst

A light-weight library targeting a [specific protocol version](https://minecraft.wiki/w/Java_Edition_1.8.9) for developing Minecraft servers.
Amethyst is customizable and sacrifices many of the vanilla features in favor of performance and memory usage.

## Getting Started

Implementing [ISubscriber](https://github.com/TheVeryStarlk/Amethyst/blob/rewrite/Amethyst.Abstractions/Eventing/ISubscriber.cs) and registering Amethyst by `services.AddAmethyst<FooSubscriber>();` is all you need to get a running Amethyst server.
However, you need to create/specify a world for players to join to.

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
Without `ChunkSubscriber`, players receive no chunks.

```csharp
services
    .AddAmethyst<FooSubscriber>()
    .AddSubscriber<ChunkSubscriber>()       // Streams chunks around players.
    .AddSubscriber<ListSubscriber>()     // Keeps the tab list in sync.
    .AddSubscriber<VisibilitySubscriber>(); // Spawns and moves players for each other.
```

## Credits

* [Minecraft Wiki](https://minecraft.wiki/w/Protocol?oldid=2772100) for the amazing documentations about the game's internal technical details.
* [Obsidian](https://github.com/ObsidianMC/Obsidian) for being an awesome reference.