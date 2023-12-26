using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace StepSys.Infrastructure.Services;

internal class DiscordStarter : IHostedService
{
    private readonly DiscordSocketClient _discord;
    private readonly IConfiguration _config;
    private readonly ILogger<DiscordSocketClient> _logger;

    public DiscordStarter(
        DiscordSocketClient discord,
        IConfiguration config,
        ILogger<DiscordSocketClient> logger)
    {
        _discord = discord;
        _config = config;
        _logger = logger;

        _discord.Log += message =>
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
        var token = _config["Discord:BotToken"]
            ?? throw new ArgumentException("Discord bot token was null.");

        await _discord.LoginAsync(TokenType.Bot, token);
        await _discord.StartAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _discord.LogoutAsync();
        await _discord.StopAsync();
    }
}