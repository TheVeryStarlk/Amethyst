using Amethyst.Abstractions;

namespace Amethyst.Eventing.Server;

public sealed record Tick : Event<IServer>;