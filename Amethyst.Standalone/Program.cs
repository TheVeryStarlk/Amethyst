using Amethyst.Extensions.Subscribers;
using Amethyst.Hosting;
using Amethyst.Standalone;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddAmethyst<StandaloneSubscriber>()
    .AddSubscriber<UsernameSubscriber>()
    .AddSubscriber<ChunkSubscriber>()
    .AddSubscriber<ListSubscriber>()
    .AddSubscriber<VisibilitySubscriber>()
    .AddSubscriber<BlockSubscriber>();

var host = builder.Build();
await host.RunAsync();