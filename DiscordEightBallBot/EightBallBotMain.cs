using System;
using System.Reflection;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
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

        public EightBallBotMain(IServiceProvider serviceProvider, ILoggerFactory loggerFactory, IOptions<BotOptions> botOptions, ILogger<EightBallBotMain> logger)
        {
            _logger = logger;

            _random = new Random();

            // Create discord client
            _discord = new DiscordClient(
                new DiscordConfiguration
                {
                    Token = botOptions.Value.DiscordToken,
                    TokenType = TokenType.Bot,
                    LoggerFactory = loggerFactory
                }
            );

            // Register event handler
            _discord.MessageCreated += HandleMessage;
        }

        private async Task HandleMessage(DiscordClient sender, MessageCreateEventArgs e)
        {
            try
            {
                // Check for 8 ball message
                if (e.Message.Content
                    .Trim()
                    .ToLower()
                    .Equals("m8!"))
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