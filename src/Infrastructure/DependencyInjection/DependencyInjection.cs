using StepSys.Infrastructure.Services;

namespace StepSys.Infrastructure.DependencyInjection;

internal static class DependencyInjection
{
    public static IServiceCollection AddDiscordBot(
        this IServiceCollection services)
    {
        services.AddSingleton(provider =>
        {
            var config = new DiscordSocketConfig
            {
                GatewayIntents =
                    GatewayIntents.AllUnprivileged
                    & ~GatewayIntents.GuildInvites
                    & ~GatewayIntents.GuildScheduledEvents,

                UseInteractionSnowflakeDate = false,
                LogLevel = LogSeverity.Debug,
            };

            var client = new DiscordSocketClient(config);

            return client;
        });

        services.AddSingleton<InteractionService>();
        services.AddHostedService<InteractionHandler>();
        services.AddHostedService<DiscordStarter>();

        return services;
    }
}
