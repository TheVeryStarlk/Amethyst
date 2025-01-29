using Amethyst.Console;
using Amethyst.Hosting;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder();

builder.Services.AddAmethyst<DefaultSubscriber>(options => options.Timeout = TimeSpan.Zero);

var host = builder.Build();

host.Run();