using Amethyst.Example;
using Amethyst.Hosting;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder();

builder.Services.AddAmethyst(options => options.AddSubscriber<Subscriber>());

builder.Build().Run();