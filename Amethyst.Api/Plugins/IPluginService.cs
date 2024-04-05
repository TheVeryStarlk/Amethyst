namespace Amethyst.Api.Plugins;

public interface IPluginService
{
    public IEnumerable<PluginInformation> Plugins { get; }
}