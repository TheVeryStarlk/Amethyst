namespace Amethyst.Abstractions.Eventing;

/// <summary>
/// An event that subscribers can cancel, once cancelled the remaining subscribers are skipped and the action is not performed.
/// </summary>
public interface ICancellable
{
    public bool IsCancelled { get; set; }
}
