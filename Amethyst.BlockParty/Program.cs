using Amethyst.BlockParty;
using Amethyst.Hosting;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder();

builder.Services.AddAmethyst<Subscriber>();

var application = builder.Build();

application.Run();