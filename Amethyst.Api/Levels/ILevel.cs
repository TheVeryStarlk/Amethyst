namespace Amethyst.Api.Levels;

public interface ILevel : IAsyncDisposable
{
    public Dictionary<string, IWorld> Worlds { get; }
}