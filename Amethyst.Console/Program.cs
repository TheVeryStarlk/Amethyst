using Amethyst.Console;
using Amethyst.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder();

// Only show Microsoft.*'s logging warnings and exceptions.
builder.Logging.AddFilter((_, category, logLevel) => !category!.Contains("Microsoft") || logLevel > LogLevel.Information);

builder.Services.AddAmethyst<DefaultSubscriber>();
builder.Services.AddTransient<AuthenticationService>();

var host = builder.Build();

host.Run();