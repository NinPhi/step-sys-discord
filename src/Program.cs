using Microsoft.Extensions.Hosting;
using StepSys.Infrastructure.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDiscordBot();

using var host = builder.Build();

await host.RunAsync();