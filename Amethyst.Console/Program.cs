using Amethyst.Console;
using Amethyst.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder();

builder.Logging.SetMinimumLevel(LogLevel.Trace);

builder.Services.AddAmethyst<DefaultSubscriber>(options => options.Timeout = TimeSpan.Zero);

var host = builder.Build();

host.Run();