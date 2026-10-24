using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Abstractions.Worlds;
using Microsoft.Extensions.Hosting;

namespace Amethyst.Example.Time;

/// <summary>
/// Amethyst has no tick loop, so this one moves the sun along and keeps the tab list's header and footer up to date.
/// </summary>
internal sealed class ClockService(Clock clock, Realm realm, IWorldFactory worldFactory) : BackgroundService
{
    // The game runs at twenty ticks a second.
    private const int Rate = 20;

    public static Message Header { get; } = Message
        .Create()
        .WriteLine("Amethyst").LightPurple().Bold()
        .Write("A procedurally generated overworld").Gray()
        .Build();

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(1));

        try
        {
            while (await timer.WaitForNextTickAsync(cancellationToken).ConfigureAwait(false))
            {
                clock.Advance(Rate);

                // Clients keep the sun moving on their own between updates.
                worldFactory.Broadcast(new TimePacket(clock.Age, clock.Time), new ListHeaderFooterPacket(Header, Footer()));
            }
        }
        catch (OperationCanceledException)
        {
            // The host is stopping.
        }
    }

    public Message Footer()
    {
        var online = worldFactory.Players().Count();

        return Chat.Of(
            $"{online} {(online is 1 ? "explorer" : "explorers")} online".Paint(Color.Yellow),
            "  |  ".Paint(Color.DarkGray),
            $"Day {clock.Age / Clock.Day + 1}, {clock.Display()}".Paint(Color.Aqua),
            "  |  ".Paint(Color.DarkGray),
            $"Seed {realm.Seed}".Paint(Color.Gray));
    }
}
