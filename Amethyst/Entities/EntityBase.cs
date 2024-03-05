using Amethyst.Api.Entities;

namespace Amethyst.Entities;

internal abstract class EntityBase : IEntity
{
    public abstract int Identifier { get; }
}