using Amethyst.Hosting;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder();

builder.Services.AddAmethyst();

var host = builder.Build();

host.Run();