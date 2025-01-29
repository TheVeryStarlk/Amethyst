using Amethyst.Components.Eventing;
using Microsoft.Extensions.Logging;

namespace Amethyst.Console;

internal sealed class DefaultSubscriber(ILogger<DefaultSubscriber> logger) : ISubscriber
{
    public void Subscribe(IRegistry registry)
    {
    }
}