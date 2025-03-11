namespace Amethyst.Abstractions.Worlds;

// Probably should have a collection of hard-coded block types.
public readonly record struct Block(int Type, int Metadata = 0);