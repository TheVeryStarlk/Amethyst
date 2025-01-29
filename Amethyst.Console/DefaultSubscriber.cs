using Amethyst.Components.Eventing;
using Microsoft.Extensions.Logging;

namespace Amethyst.Console;

internal sealed class DefaultSubscriber(ILogger<DefaultSubscriber> logger) : Subscriber
{
    public override void Subscribe(IRegistry registry)
    {
    }
}