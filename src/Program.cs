using Microsoft.Extensions.Hosting;
using StepSys.Infrastructure.DependencyInjection;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDiscordBot();
builder.Services.AddMediator();
builder.Services.AddDatabase(builder.Configuration);

using var host = builder.Build();

await host.RunAsync();