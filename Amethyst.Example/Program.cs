using Amethyst.Example;
using Amethyst.Extensions.Subscribers;
using Amethyst.Hosting;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddAmethyst<ExampleSubscriber>()
    .AddSubscriber<UsernameSubscriber>()
    .AddSubscriber<ChunkSubscriber>()
    .AddSubscriber<ListSubscriber>()
    .AddSubscriber<VisibilitySubscriber>()
    .AddSubscriber<BlockSubscriber>();

var host = builder.Build();
await host.RunAsync();
