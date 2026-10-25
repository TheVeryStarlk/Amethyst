using Amethyst.Abstractions.Entities;
using Amethyst.Abstractions.Entities.Player;
using Amethyst.Abstractions.Eventing;
using Amethyst.Abstractions.Eventing.Player;
using Amethyst.Abstractions.Messages;
using Amethyst.Abstractions.Packets.Play;
using Amethyst.Abstractions.Worlds;
using Amethyst.Example.Creatures;
using Amethyst.Example.Generation;
using Amethyst.Example.Time;
using Amethyst.Extensions;

namespace Amethyst.Example.Subscribers;

/// <summary>
/// Chat commands, with tab completion for their names and arguments.
/// </summary>
internal sealed class CommandSubscriber(Realm realm, Clock clock, IWorldFactory worldFactory, Bestiary bestiary, Ballistics ballistics, Showcase showcase, Pets pets) : ISubscriber
{
    private static readonly (string Name, string Usage, string Description)[] commands =
    [
        ("help", "/help", "Shows this list."),
        ("spawn", "/spawn", "Takes you back to the spawn."),
        ("top", "/top", "Climbs to the highest block above you."),
        ("explore", "/explore <biome>", "Takes you to the nearest biome of a kind."),
        ("biome", "/biome", "Names the biome you are standing in."),
        ("tree", "/tree [kind]", "Grows a tree right in front of you."),
        ("time", "/time [day|noon|night|midnight]", "Shows or changes the time of day."),
        ("weather", "/weather <clear|rain>", "Changes the weather for everyone."),
        ("tp", "/tp <player>", "Takes you to another explorer."),
        ("kit", "/kit", "Refills your hotbar with the wands."),
        ("seed", "/seed", "Shows the world's seed."),
        ("list", "/list", "Lists everyone online."),
        ("summon", "/summon <entity> [count]", "Summons any entity where you look."),
        ("zoo", "/zoo", "Surrounds you with every mob, name tags included."),
        ("pet", "/pet <mob|dismiss>", "Adopts any mob as a companion."),
        ("carousel", "/carousel", "Builds a merry-go-round of horses ahead."),
        ("dance", "/dance", "Opens a dance floor of armor stands ahead."),
        ("gallery", "/gallery", "Hangs every painting on a wall ahead."),
        ("party", "/party", "Rains rainbow sheep, fireworks and experience."),
        ("smite", "/smite [player]", "Strikes lightning on a player, or where you look."),
        ("entities", "/entities", "Counts the entities in the world."),
        ("clear", "/clear [radius]", "Despawns the entities around you, except pets.")
    ];

    private static readonly Dictionary<string, long> times = new()
    {
        ["day"] = 1000,
        ["noon"] = 6000,
        ["night"] = 13000,
        ["midnight"] = 18000
    };

    public void Subscribe(IRegistry registry)
    {
        registry.For<IPlayer>(consumer =>
        {
            consumer.On<Sent>((player, sent) =>
            {
                if (sent.Message.StartsWith('/'))
                {
                    Execute(player, sent.Message[1..].Split(' ', StringSplitOptions.RemoveEmptyEntries));
                }
            });

            consumer.On<Tab>((_, tab) =>
            {
                // The client replaces the last word with the match.
                var words = tab.Behind.Split(' ');

                // Amethyst prefixes the matches of a single word with a slash, so only complete it for commands.
                if (words.Length is 1)
                {
                    tab.Matches = tab.Behind.StartsWith('/')
                        ? commands.Select(command => command.Name).Where(name => name.StartsWith(tab.Behind[1..], StringComparison.OrdinalIgnoreCase)).ToArray()
                        : [];

                    return;
                }

                var candidates = tab.Behind.StartsWith('/') ? Suggest(words[0][1..].ToLowerInvariant(), words.Length - 2) : Usernames();
                tab.Matches = candidates.Where(candidate => candidate.StartsWith(words[^1], StringComparison.OrdinalIgnoreCase)).ToArray();
            });
        });
    }

    private IEnumerable<string> Suggest(string command, int argument)
    {
        if (argument > 0)
        {
            return [];
        }

        return command switch
        {
            "explore" => OverworldGenerator.Biomes.Select(Name),
            "tree" => Enum.GetValues<TreeKind>().Select(kind => kind.ToString().ToLowerInvariant()),
            "time" => times.Keys,
            "weather" => ["clear", "rain"],
            "tp" => Usernames(),
            "summon" => bestiary.Names,
            "pet" => ["dismiss", .. bestiary.Mobs],
            "smite" => Usernames(),
            _ => []
        };
    }

    private void Execute(IPlayer player, string[] arguments)
    {
        var command = arguments.Length > 0 ? arguments[0].ToLowerInvariant() : string.Empty;
        var argument = arguments.Length > 1 ? arguments[1].ToLowerInvariant() : null;

        switch (command)
        {
            case "help":
                foreach (var (_, usage, description) in commands)
                {
                    player.Send(Chat.Of(usage.Paint(Color.LightPurple), $" {description}".Paint(Color.Gray)));
                }

                break;

            case "spawn":
                player.Teleport(realm.Spawn);
                Reply(player, "Welcome back to the spawn.", Color.Green);
                break;

            case "top":
                player.Teleport(realm.Surface((int) Math.Floor(player.Position.X), (int) Math.Floor(player.Position.Z)));
                break;

            case "explore":
                Explore(player, argument);
                break;

            case "biome":
                player.Send(Chat.Of("You are in the ".Paint(Color.Gray), realm.BiomeAt(player.Position).Humanize().Paint(Color.Green), ".".Paint(Color.Gray)));
                break;

            case "tree":
                Plant(player, argument);
                break;

            case "time":
                Time(player, argument);
                break;

            case "weather" when argument is "clear" or "rain":
                clock.Raining = argument is "rain";

                worldFactory.Broadcast(new ChangeStatePacket(clock.Raining ? new StartRainingState() : new StopRainingState()));
                worldFactory.Broadcast(Chat.Of(player.Username.Paint(ChatSubscriber.Tint(player)), (clock.Raining ? " summoned the rain." : " cleared the skies.").Paint(Color.Gray)));

                break;

            case "tp":
                Visit(player, argument);
                break;

            case "kit":
                Kit.Give(player);
                Reply(player, "Your hotbar has been refilled.", Color.Green);
                break;

            case "seed":
                player.Send(Chat.Of("Seed: ".Paint(Color.Gray), realm.Seed.ToString().Paint(Color.Aqua)));
                break;

            case "list":
                var players = worldFactory.Players().ToArray();
                var names = players.SelectMany((other, index) => new[] { other.Username.Paint(ChatSubscriber.Tint(other)), (index < players.Length - 1 ? ", " : string.Empty).Paint(Color.Gray) });

                player.Send(Chat.Of([$"Online ({players.Length}): ".Paint(Color.Gray), .. names]));
                break;

            case "summon":
                Summon(player, argument, arguments.Length > 2 ? arguments[2] : null);
                break;

            case "zoo":
                Reply(player, $"Welcome to the zoo, all {showcase.Zoo(player)} mobs are here!", Color.Green);
                break;

            case "pet":
                Adopt(player, argument);
                break;

            case "carousel":
                showcase.Carousel(player);
                Reply(player, "Step right up, the carousel is spinning!", Color.Green);
                break;

            case "dance":
                showcase.Dance(player);
                Reply(player, "The dance floor is open!", Color.Green);
                break;

            case "gallery":
                showcase.Gallery(player);
                Reply(player, "The gallery is open, every painting is on the wall.", Color.Green);
                break;

            case "party":
                showcase.Party(player);
                worldFactory.Broadcast(Chat.Of(player.Username.Paint(ChatSubscriber.Tint(player)), " is throwing a party!".Paint(Color.LightPurple)));
                break;

            case "smite":
                Smite(player, argument);
                break;

            case "entities":
                Census(player);
                break;

            case "clear":
                Clear(player, argument);
                break;

            default:
                var known = commands.FirstOrDefault(candidate => candidate.Name == command);

                Reply(player, known.Name is null ? $"Unknown command \"/{command}\", try /help." : $"Usage: {known.Usage}", Color.Red);
                break;
        }
    }

    private void Explore(IPlayer player, string? argument)
    {
        var index = Array.FindIndex(OverworldGenerator.Biomes, candidate => Name(candidate) == argument);

        if (index < 0)
        {
            Reply(player, "Usage: /explore <biome>, press tab to see them all.", Color.Red);
            return;
        }

        var biome = OverworldGenerator.Biomes[index];

        var x = (int) Math.Floor(player.Position.X);
        var z = (int) Math.Floor(player.Position.Z);

        if (realm.Find(biome, x, z) is not { } found)
        {
            Reply(player, $"There is no {biome.Humanize()} anywhere close.", Color.Red);
            return;
        }

        player.Teleport(realm.Surface(found.X, found.Z));

        var distance = (int) Math.Sqrt((found.X - x) * (double) (found.X - x) + (found.Z - z) * (double) (found.Z - z));
        player.Send(Chat.Of("Welcome to the ".Paint(Color.Gray), biome.Humanize().Paint(Color.Green), $", {distance} blocks from where you were.".Paint(Color.Gray)));
    }

    private void Plant(IPlayer player, string? argument)
    {
        TreeKind kind;

        if (argument is null)
        {
            kind = Trees.Pick(realm.BiomeAt(player.Position), Random.Shared);
        }
        else if (!Enum.TryParse(argument, true, out kind) || !Enum.IsDefined(kind))
        {
            Reply(player, $"Usage: /tree [{string.Join('|', Enum.GetNames<TreeKind>()).ToLowerInvariant()}]", Color.Red);
            return;
        }

        // A few blocks ahead, so the tree does not grow through the player.
        var yaw = player.Yaw * Math.PI / 180;

        var x = (int) Math.Floor(player.Position.X - Math.Sin(yaw) * 5);
        var z = (int) Math.Floor(player.Position.Z + Math.Cos(yaw) * 5);

        Trees.Grow(new WorldCanvas(player.World), kind, x, (int) realm.Surface(x, z).Y, z, Random.Shared);
    }

    private void Time(IPlayer player, string? argument)
    {
        if (argument is null)
        {
            player.Send(Chat.Of("It is ".Paint(Color.Gray), clock.Display().Paint(Color.Aqua), $" on day {clock.Age / Clock.Day + 1}.".Paint(Color.Gray)));
            return;
        }

        if (!times.TryGetValue(argument, out var time) && !long.TryParse(argument, out time))
        {
            Reply(player, "Usage: /time [day|noon|night|midnight|<ticks>]", Color.Red);
            return;
        }

        clock.Time = time;

        worldFactory.Broadcast(new TimePacket(clock.Age, clock.Time));
        worldFactory.Broadcast(Chat.Of(player.Username.Paint(ChatSubscriber.Tint(player)), " turned the clock to ".Paint(Color.Gray), clock.Display().Paint(Color.Aqua), ".".Paint(Color.Gray)));
    }

    private void Visit(IPlayer player, string? argument)
    {
        var target = worldFactory.Players().FirstOrDefault(other => other.Username.Equals(argument, StringComparison.OrdinalIgnoreCase));

        if (target is null || target == player)
        {
            Reply(player, "Usage: /tp <player>, press tab to see who is online.", Color.Red);
            return;
        }

        if (target.World == player.World)
        {
            player.Teleport(target.Position);
        }
        else
        {
            player.Spawn(target.World, target.Position);
        }

        Reply(player, $"Visiting {target.Username}.", Color.Green);
    }

    private void Summon(IPlayer player, string? argument, string? amount)
    {
        if (argument is null || !bestiary.Names.Contains(argument))
        {
            Reply(player, "Usage: /summon <entity> [count], press tab to see them all.", Color.Red);
            return;
        }

        var count = Math.Clamp(int.TryParse(amount, out var parsed) ? parsed : 1, 1, 16);
        var direction = Motion.Direction(player.Yaw, player.Pitch);

        // Where the player looks, or a few blocks ahead on the ground if they look at the sky.
        var target = Motion.Trace(player.World, Motion.Eye(player), direction, 32) ?? Ahead(player, 6);

        for (var index = 0; index < count; index++)
        {
            // Crowds spread out around the first one.
            var spot = index is 0 ? target : target with { X = target.X + Random.Shared.NextDouble() * 4 - 2, Z = target.Z + Random.Shared.NextDouble() * 4 - 2 };
            bestiary.Summon(argument, new Summoning(player.World, spot, player.Yaw, player.Pitch, player));
        }

        player.Send(Chat.Of("Summoned ".Paint(Color.Gray), $"{count} × ".Paint(Color.Aqua), Bestiary.Humanize(argument).Paint(Color.Green), ".".Paint(Color.Gray)), MessagePosition.HotBar);
    }

    private void Adopt(IPlayer player, string? argument)
    {
        if (argument is "dismiss")
        {
            Reply(player, pets.Dismiss(player) ? "Your pet went home." : "You have no pet to send home.", Color.Gray);
            return;
        }

        if (argument is null || pets.Adopt(player, argument) is null)
        {
            Reply(player, "Usage: /pet <mob|dismiss>, press tab to see every mob.", Color.Red);
            return;
        }

        player.Send(Chat.Of("Meet your new ".Paint(Color.Gray), Bestiary.Humanize(argument).Paint(Color.Green), ", it will follow you everywhere.".Paint(Color.Gray)));
    }

    private void Smite(IPlayer player, string? argument)
    {
        if (argument is null)
        {
            var target = Motion.Trace(player.World, Motion.Eye(player), Motion.Direction(player.Yaw, player.Pitch), 128);

            if (target is null)
            {
                Reply(player, "The storm cannot reach that far.", Color.Red);
                return;
            }

            ballistics.Strike(player.World, target.Value);
            return;
        }

        var victim = worldFactory.Players().FirstOrDefault(other => other.Username.Equals(argument, StringComparison.OrdinalIgnoreCase));

        if (victim is null)
        {
            Reply(player, "Usage: /smite [player], press tab to see who is online.", Color.Red);
            return;
        }

        ballistics.Strike(victim.World, victim.Position);
        worldFactory.Broadcast(Chat.Of(player.Username.Paint(ChatSubscriber.Tint(player)), " smote ".Paint(Color.Gray), victim.Username.Paint(ChatSubscriber.Tint(victim)), "!".Paint(Color.Gray)));
    }

    private void Census(IPlayer player)
    {
        var census = showcase.Census(player.World);
        var total = census.Sum(pair => pair.Count);

        if (total is 0)
        {
            Reply(player, "There are no entities in this world, try /summon or /zoo.", Color.Gray);
            return;
        }

        var parts = census.SelectMany((pair, index) => new[]
        {
            $"{pair.Count} ".Paint(Color.Aqua),
            pair.Kind.Paint(Color.Green),
            (index < census.Length - 1 ? ", " : string.Empty).Paint(Color.Gray)
        });

        player.Send(Chat.Of([$"Entities ({total}): ".Paint(Color.Gray), .. parts]));
    }

    private void Clear(IPlayer player, string? argument)
    {
        var radius = Math.Clamp(double.TryParse(argument, out var parsed) ? parsed : 48, 1, 512);
        var cleared = showcase.Clear(player.World, player.Position, radius, pets.IsPet);

        Reply(player, $"Despawned {cleared} {(cleared is 1 ? "entity" : "entities")} within {radius} blocks.", Color.Green);
    }

    private Position Ahead(IPlayer player, double distance)
    {
        var ahead = Motion.Along(player.Position, Motion.Direction(player.Yaw, 0), distance);
        return realm.Surface((int) Math.Floor(ahead.X), (int) Math.Floor(ahead.Z));
    }

    private IEnumerable<string> Usernames()
    {
        return worldFactory.Players().Select(player => player.Username);
    }

    private static string Name(Biome biome)
    {
        return biome.Humanize().Replace(' ', '_').ToLowerInvariant();
    }

    private static void Reply(IPlayer player, string text, Color color)
    {
        player.Send(text.Paint(color));
    }
}
