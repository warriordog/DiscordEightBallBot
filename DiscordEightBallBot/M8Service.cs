using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscordEightBallBot;

public class M8Service : IHostedService
{
    private readonly DiscordClient _discord;
    private readonly ILogger<M8Service> _logger;

    public M8Service(ILoggerFactory loggerFactory, IOptions<BotOptions> botOptions, ILogger<M8Service> logger, IServiceProvider services)
    {
        _logger = logger;

        // Create discord client
        _discord = new DiscordClient(
            new DiscordConfiguration
            {
                Token = botOptions.Value.DiscordToken,
                TokenType = TokenType.Bot,
                LoggerFactory = loggerFactory,
                Intents = DiscordIntents.Guilds
            }
        );
        
        // Configure slash commands
        var slashConfig = new SlashCommandsConfiguration
        {
            Services = services
        };
        var slash = _discord.UseSlashCommands(slashConfig);
        slash.RegisterCommands<M8Commands>(); // TODO implement single-server registration for debugging 
        
        // Log when bot joins a server.
        _discord.GuildCreated += (_, e) =>
        {
            _logger.LogInformation("Joined server {GuildId} ({GuildName})", e.Guild.Id, e.Guild.Name);
            return Task.CompletedTask;
        };
    }

    public async Task StartAsync(CancellationToken _)
    {
        _logger.LogInformation("8Bot is starting with version {Version}.", GetType().Assembly.GetName().Version);
        await _discord.ConnectAsync();
        _logger.LogInformation($"8Bot has started.");
    }

    public async Task StopAsync(CancellationToken _)
    {
        _logger.LogInformation("8Bot is stopping.");
        await _discord.DisconnectAsync();
        _logger.LogInformation("8Bot has stopped.");
    }
}

public class BotOptions
{
    [Required]
    public string DiscordToken { get; init; } = "";
}