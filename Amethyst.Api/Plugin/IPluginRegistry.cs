using Amethyst.Api.Plugin.Events;

namespace Amethyst.Api.Plugin;

public interface IPluginRegistry
{
    public void RegisterEvent<T>(GenericDelegate<T> @delegate) where T : MinecraftEventArgsBase;
}

public delegate Task GenericDelegate<in T>(T @event) where T : MinecraftEventArgsBase;