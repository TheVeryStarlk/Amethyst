using Amethyst.Abstractions.Entities.Player;

namespace Amethyst.Eventing.Player;

// Any inventory-related change. Think of a better name.
public sealed class Inventory : IEvent<IPlayer>;