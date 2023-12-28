using StepSys.Infrastructure.Data;
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
        services.AddHostedService<DiscordBotStarter>();

        return services;
    }

    public static IServiceCollection AddMediator(
        this IServiceCollection services)
    {
        services.AddMediatR(config =>
            config.RegisterServicesFromAssemblyContaining<Program>());

        return services;
    }

    public static IServiceCollection AddDatabase(
        this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(opts =>
            opts.UseSqlite(configuration.GetConnectionString("SqliteConnection")));

        return services;
    }
}
