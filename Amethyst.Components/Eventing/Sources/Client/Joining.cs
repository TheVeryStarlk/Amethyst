namespace Amethyst.Components.Eventing.Sources.Client;

public sealed class Joining(string username) : Event<IClient>
{
    public string Username => username;

    public byte GameMode { get; set; }

    public byte MaximumPlayerCount { get; set; }

    public bool ReducedDebugInformation { get; set; }

    public double X { get; set; }

    public double Y { get; set; }

    public double Z { get; set; }

    public float Yaw { get; set; }

    public float Pitch { get; set; }
}