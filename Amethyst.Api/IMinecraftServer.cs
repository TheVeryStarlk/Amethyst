using Amethyst.Api.Components;

namespace Amethyst.Api;

public interface IMinecraftServer : IAsyncDisposable
{
    public ServerStatus Status { get; }
}