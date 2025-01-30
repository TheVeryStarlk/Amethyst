<p align="center">
  <img width="100" height="100" align="center" src="https://i.imgur.com/BkTfea4.png">
</p>

# Amethyst

A low-level implementation of the Minecraft Java edition server protocol.\
Amethyst is customizable and sacrifices many of the vanilla mechanics in favor of performance and memory usage.

## Structure

Amethyst by nature has very little built-in logic, the way you implement logic to Amethyst is by subscribing to events.\
Each part of Amethyst has a collection of events that you can subscribe to.

* Amethyst
    * The main project, where the actual implementation of the server and related parts resides.
    * Contains important components like worlds, event dispatching and the server itself.
* Amethyst.Components (Might rename to Amethyst.Abstraction)
    * Contains small, commonly used components such as the chat messages and server status POCOs.
* Amethyst.Protocol
    * The implementation of the protocol and all packets reside in that project.
    * This might get merged into Amethyst.Components.
* Amethyst.Console
    * An example console project of Amethyst's API.

This is all subject to change as Amethyst develops, expect big rewrites and refactors.

## Credits

* [Minecraft Wiki](https://minecraft.wiki/w/Protocol?oldid=2772100) for the amazing documentations about the game's internal technical details.
* [Obsidian](https://github.com/ObsidianMC/Obsidian) for being an awesome reference.