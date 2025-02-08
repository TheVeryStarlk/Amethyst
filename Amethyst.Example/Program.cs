using Amethyst.Example;
using Amethyst.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder();

builder.Logging.SetMinimumLevel(LogLevel.Trace);

builder.Services.AddAmethyst(options => options.AddSubscriber<Subscriber>());

builder.Build().Run();