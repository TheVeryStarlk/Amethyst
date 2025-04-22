using Amethyst.Abstractions;
using Amethyst.Abstractions.Messages;

namespace Amethyst.Eventing.Client;

public sealed class Request : IEvent<IClient>
{
    public string Name { get; set; } = "Amethyst";

    public int Numerical { get; set; } = 47;

    public int Maximum { get; set; } = byte.MaxValue;

    public int Online { get; set; } = 0;

    public Message Description { get; set; } = Message.Simple("Hello world!");

    public string Favicon { get; set; } = string.Empty;
}