using Amethyst.Abstractions;
using Amethyst.Abstractions.Messages;

namespace Amethyst.Eventing.Client;

public sealed class Status : Event<IClient>
{
    public string Name { get; set; } = "Amethyst";

    public int Numerical { get; set; } = 47;

    public int Maximum { get; set; } = 10;

    public int Online { get; set; } = 0;

    public Message Description { get; set; } = Message.Simple("Hello world!");

    public string Favicon { get; set; } = string.Empty;
}