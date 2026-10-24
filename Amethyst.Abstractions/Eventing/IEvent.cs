namespace Amethyst.Abstractions.Eventing;

// Create an ISource and constraint the T to it.
// Contravariant so an event declared for an abstraction (e.g. IClient) can be dispatched from its implementation.
public interface IEvent<in T>;
