using Amethyst.Components.Worlds;

namespace Amethyst.Worlds;

internal static class PositionExtensions
{
    public static Position ToRegion(this Position position)
    {
        return new Position(position.X >> 5, position.Y >> 5, position.Z >> 5);
    }

    public static Position ToChunk(this Position position)
    {
        return new Position(position.X >> 4, position.Y >> 4, position.Z >> 4);
    }

    public static Position ToSection(this Position position)
    {
        return new Position(position.X % 16, position.Y % 16, position.Z % 16);
    }
}