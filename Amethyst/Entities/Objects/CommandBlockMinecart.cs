using Amethyst.Abstractions.Entities.Objects;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Worlds;

namespace Amethyst.Entities.Objects;

internal sealed class CommandBlockMinecart(IWorld world) : Minecart(world), ICommandBlockMinecart
{
    public string Command { get; set; } = string.Empty;

    public Message? LastOutput { get; set; }
}