<p align="center">
  <img width="100" height="100" align="center" src="https://i.imgur.com/BkTfea4.png">
</p>

# Amethyst

A light-weight implementation of the Minecraft Java edition server protocol.
Amethyst is customizable and sacrifices many of the vanilla mechanics in favor of performance and memory usage.

## Structure

Amethyst by nature has very little built-in logic, the way you implement logic to Amethyst is by subscribing to events.

```csharp
registry.For<IPlayer>(consumer =>
{
    consumer.On<Joined>(async (player, _, _) =>
    {
        var message = Message.Create("Welcome!", color: Color.Yellow);
        await player.SendAsync(message, MessagePosition.Box);
    });
});
```

The Amethyst.Console projects serves as an extensive example of usage of the event API.\
Be careful that everything is currently subject to change as Amethyst develops.

## Roadmap

This is not by any means a complete roadmap. There's still a lot to do.

* Server list ping protocol
    * Implement legacy ping packets.
* Networking
    * Implement encryption and compression.
* Worlds
    * Design an API for creating, saving and modifying worlds.

## Credits

* [Minecraft Wiki](https://minecraft.wiki/w/Protocol?oldid=2772100) for the amazing documentations about the game's internal technical details.
* [Obsidian](https://github.com/ObsidianMC/Obsidian) for being an awesome reference.