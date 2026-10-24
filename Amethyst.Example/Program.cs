using Amethyst.Example;
using Amethyst.Example.Subscribers;
using Amethyst.Example.Time;
using Amethyst.Extensions.Subscribers;
using Amethyst.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

// The example's own state, shared between its subscribers and the clock.
builder.Services
    .AddSingleton<Realm>()
    .AddSingleton<Clock>()
    .AddHostedService<ClockService>();

// Subscribers run in the order they were registered, so the bedrock protection in ServerSubscriber
// gets to cancel digging before BlockSubscriber applies it to the world.
builder.Services
    .AddAmethyst<ServerSubscriber>()
    .AddSubscriber<ChatSubscriber>()
    .AddSubscriber<CommandSubscriber>()
    .AddSubscriber<WandSubscriber>()
    .AddSubscriber<CompassSubscriber>()
    .AddSubscriber<UsernameSubscriber>()
    .AddSubscriber<ChunkSubscriber>()
    .AddSubscriber<ListSubscriber>()
    .AddSubscriber<VisibilitySubscriber>()
    .AddSubscriber<BlockSubscriber>();

var host = builder.Build();
await host.RunAsync();
