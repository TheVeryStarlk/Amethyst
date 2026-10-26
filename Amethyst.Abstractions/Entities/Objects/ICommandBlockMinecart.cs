using Amethyst.Abstractions.Messages;

namespace Amethyst.Abstractions.Entities.Objects;

public interface ICommandBlockMinecart : IMinecart
{
    public string Command { get; set; }

    public Message? LastOutput { get; set; }
}