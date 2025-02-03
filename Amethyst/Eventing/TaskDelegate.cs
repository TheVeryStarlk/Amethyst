namespace Amethyst.Eventing;

public delegate Task TaskDelegate<in TSource, in TEvent>(TSource source, TEvent original, CancellationToken cancellationToken);