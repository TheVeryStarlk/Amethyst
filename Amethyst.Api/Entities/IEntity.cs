namespace Amethyst.Api.Entities;

public interface IEntity
{
    public int Identifier { get; }

    public Position Position { get; set; }

    public float Yaw { get; set; }

    public float Pitch { get; set; }

    public bool OnGround { get; set; }

    public Task TeleportAsync(Position position);
}