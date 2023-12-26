using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace StepSys.Infrastructure.Services;

internal class InteractionHandler : IHostedService
{
    private readonly DiscordSocketClient _discord;
    private readonly InteractionService _interactions;
    private readonly IServiceProvider _services;
    private readonly IConfiguration _config;
    private readonly ILogger<InteractionService> _logger;

    public InteractionHandler(
        DiscordSocketClient discord,
        InteractionService interactions,
        IServiceProvider services,
        IConfiguration config,
        ILogger<InteractionService> logger)
    {
        _discord = discord;
        _interactions = interactions;
        _services = services;
        _config = config;
        _logger = logger;

        _interactions.Log += message =>
        {
            var logLevel = message.Severity switch
            {
                LogSeverity.Critical => LogLevel.Critical,
                LogSeverity.Error => LogLevel.Error,
                LogSeverity.Warning => LogLevel.Warning,
                LogSeverity.Info => LogLevel.Information,
                LogSeverity.Verbose => LogLevel.Trace,
                LogSeverity.Debug => LogLevel.Debug,
                _ => LogLevel.Information,
            };

            _logger.Log(logLevel, "[Discord.NET] {Message}", message.ToString());

            return Task.CompletedTask;
        };
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        ulong[] guilds = _config.GetRequiredSection("Discord:Guilds")
            .Get<ulong[]>() ?? Array.Empty<ulong>();

        _discord.Ready += async () =>
        {
            foreach (var guild in guilds)
            {
                await _interactions.RegisterCommandsToGuildAsync(guild, true);
            }
        };

        _discord.InteractionCreated += OnInteractionAsync;

        await _interactions.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _interactions.Dispose();
        return Task.CompletedTask;
    }

    private async Task OnInteractionAsync(SocketInteraction interaction)
    {
        try
        {
            var context = new SocketInteractionContext(_discord, interaction);
            var result = await _interactions.ExecuteCommandAsync(context, _services);

            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ToString());
        }
        catch
        {
            if (interaction.Type == InteractionType.ApplicationCommand)
            {
                await interaction.GetOriginalResponseAsync()
                    .ContinueWith(msg => msg.Result.DeleteAsync());
            }
        }
    }
}