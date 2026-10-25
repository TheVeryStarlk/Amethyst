namespace Amethyst.Abstractions.Entities.Mobs;

public interface IOcelot : ITameable
{
    public OcelotType Type { get; set; }
}

public enum OcelotType
{
    Wild,
    Tuxedo,
    Tabby,
    Siamese
}