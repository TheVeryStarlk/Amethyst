namespace Amethyst.Api;

public sealed class ServerOptions
{
    public TimeSpan IdleTimeOut { get; init; } = TimeSpan.FromSeconds(5);
}