using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscordEightBallBot
{
    public class EightBallBotMain
    {
        private readonly DiscordClient             _discord;
        private readonly ILogger<EightBallBotMain> _logger;
        private readonly Random                    _random;

        public EightBallBotMain(ILoggerFactory loggerFactory, IOptions<BotOptions> botOptions, ILogger<EightBallBotMain> logger)
        {
            _logger = logger;

            _random = new Random();

            // Create discord client
            _discord = new DiscordClient(
                new DiscordConfiguration
                {
                    Token = botOptions.Value.DiscordToken,
                    TokenType = TokenType.Bot,
                    LoggerFactory = loggerFactory,
                    Intents = DiscordIntents.DirectMessages | DiscordIntents.GuildMessages | DiscordIntents.Guilds
                }
            );

            // Register event handler
            _discord.MessageCreated += HandleMessage;
            
            // Log when bot joins a server.
            _discord.GuildCreated += (d, e) => 
            {
                _logger.LogInformation($"Joined server {e.Guild.Id} ({e.Guild.Name})");
                return Task.CompletedTask;
            };
        }

        private async Task HandleMessage(DiscordClient sender, MessageCreateEventArgs e)
        {
            try
            {
                // Don't reply to ourself
                if (e.Author.Equals(sender.CurrentUser))
                {
                    return;
                }
                
                // Check permissions
                if (!e.Channel.IsPrivate)
                {
                    // Get current member (user from current channel)
                    var currentMember = e.Channel.Users.FirstOrDefault(mbr => mbr.Equals(sender.CurrentUser));
                    
                    // Check for chat permissions
                    if (currentMember == null || !e.Channel.PermissionsFor(currentMember).HasPermission(Permissions.SendMessages))
                    {
                        return;
                    }
                }
                
                // Check for 8 ball message
                if (e.Message.Content.ToLower().Contains("m8!"))
                {
                    // Get random response
                    var eightBallMessage = GetRandomMessage();

                    // Send response
                    await e.Message.RespondAsync(eightBallMessage);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in message handler");
            }
        }

        private string GetRandomMessage()
        {
            var answerIdx = _random.Next(EightBallAnswers.Length);
            return EightBallAnswers[answerIdx];
        }
        
        public async Task StartAsync()
        {
            _logger.LogInformation($"EightBallBot {GetType().Assembly.GetName().Version} starting");
            await _discord.ConnectAsync();
        }

        public Task StopAsync()
        {
            _logger.LogInformation("EightBallBot stopping");
            return _discord.DisconnectAsync();
        }

        private static readonly string[] EightBallAnswers = {
            "It is certain.", 
            "It is decidedly so.",
            "Without a doubt.",
            "Yes – definitely.",
            "You may rely on it.",
            "As I see it, yes.",
            "Most likely.",
            "Outlook good.",
            "Yes.",
            "Signs point to yes.",
            "Reply hazy, try again.",
            "Ask again later.",
            "Better not tell you now.",
            "Cannot predict now.",
            "Concentrate and ask again.",
            "Don't count on it.",
            "My reply is no.",
            "My sources say no.",
            "Outlook not so good.",
            "Very doubtful." 
        };
    }
}