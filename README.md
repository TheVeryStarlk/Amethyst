<p align="center">
  <img width="100" height="100" align="center" src="https://i.imgur.com/BkTfea4.png">
</p>

# Amethyst

A light-weight implementation of the Minecraft Java edition server protocol.
Amethyst is customizable and sacrifices many of the vanilla mechanics in favor of performance and memory usage.

## Usage

Amethyst by nature has very little built-in logic, the way you implement logic is by subscribing to events.

```csharp
registry.For<Player>(consumer => consumer.On<Joined>(async (player, _, _) =>
{
    var message = Message.Create("Welcome!", color: Color.Yellow);
    await player.SendAsync(message, MessagePosition.Box);
}));
```

## Credits

* [Minecraft Wiki](https://minecraft.wiki/w/Protocol?oldid=2772100) for the amazing documentations about the game's internal technical details.
* [Obsidian](https://github.com/ObsidianMC/Obsidian) for being an awesome reference.