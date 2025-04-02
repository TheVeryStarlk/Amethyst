<p align="center">
  <img width="100" height="100" align="center" src="https://i.imgur.com/BkTfea4.png">
</p>

# Amethyst

A light-weight library targeting a [specific protocol version](https://minecraft.wiki/w/Java_Edition_1.8.9) for developing Minecraft servers.
Amethyst is customizable and sacrifices many of the vanilla features in favor of performance and memory usage.

## Getting Started

Implementing [ISubscriber](https://github.com/TheVeryStarlk/Amethyst/blob/rewrite/Amethyst/Eventing/ISubscriber.cs) and registering Amethyst by `services.AddAmethyst<FooSubscriber>();` is all you need to get a running Amethyst server.
However, you need to create/specify a world for players to join to.

```cs
public void Subscribe(IRegistry registry)
{
    // Create an empty over world named default and assign it to a variable.
    registry.For<IServer>(consumer => consumer.On<Starting>((server, _) => server.Create("Default", WorldType.Default, Dimension.OverWorld, Difficulty.Peaceful, EmptyGenerator.Instance)));
   
    // When a client attempts to join, assign the joining world to the world we created.
    registry.For<IClient>(consumer => consumer.On<Login>((_, login) => login.World = world!));
}
```

## Usage

Amethyst by nature has very little built-in logic, the way you implement logic is by subscribing to events.
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