using Amethyst.Example;
using Amethyst.Extensions.Subscribers;
using Amethyst.Hosting;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddAmethyst<ExampleSubscriber>()
    .AddSubscriber<ChunkSubscriber>()
    .AddSubscriber<ListSubscriber>()
    .AddSubscriber<VisibilitySubscriber>();

var host = builder.Build();
await host.RunAsync();
