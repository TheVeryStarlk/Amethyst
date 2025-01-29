namespace Amethyst.Components.Eventing;

public delegate Task TaskDelegate<in TSource, in TEvent>(TSource source, TEvent original, CancellationToken cancellationToken);