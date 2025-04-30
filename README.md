<p align="center">
  <img width="100" height="100" align="center" src="https://i.imgur.com/BkTfea4.png">
</p>

> This branch is solely for showing Sloth what Amethyst is currently capable of. 
There still is quite a lot of work to be done, but here's a small block-party game written completely in Amethyst.

# Amethyst

A light-weight library targeting a [specific protocol version](https://minecraft.wiki/w/Java_Edition_1.8.9) for developing Minecraft servers.
Amethyst is customizable and sacrifices many of the vanilla features in favor of performance and memory usage.

## Getting Started

Implementing [ISubscriber](https://github.com/TheVeryStarlk/Amethyst/blob/rewrite/Amethyst/Eventing/ISubscriber.cs) and registering Amethyst by `services.AddAmethyst<FooSubscriber>();` is all you need to get a running Amethyst server.
However, you need to create/specify a world for players to join to.

```cs
registry.For<IClient>(consumer => consumer.On<Joining>((_, joining) =>
{
    var world = worldFactory.Create("Empty", EmptyGenerator.Instance);
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

## Credits

* [Minecraft Wiki](https://minecraft.wiki/w/Protocol?oldid=2772100) for the amazing documentations about the game's internal technical details.
* [Obsidian](https://github.com/ObsidianMC/Obsidian) for being an awesome reference.