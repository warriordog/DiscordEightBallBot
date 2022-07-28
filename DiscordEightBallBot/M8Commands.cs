using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscordEightBallBot;

// ReSharper disable once ClassNeverInstantiated.Global
public class M8Commands : ApplicationCommandModule
{
    private readonly M8CommandOptions _options;
    private readonly ILogger<M8Commands> _logger;
    private readonly Random _random = new();

    public M8Commands(IOptions<M8CommandOptions> options, ILogger<M8Commands> logger)
    {
        _logger = logger;
        _options = options.Value;
    }

    [SlashCommand("m8", "Ask the 8-ball")]
    public async Task M8Command(InteractionContext ctx)
    {
        using var _ = _logger.BeginScope(nameof(M8Command));
        try
        {
            _logger.LogDebug("Invoked by [{user}] in [{guild}]", ctx.User, ctx.Guild);

            // Get random response
            var eightBallMessage = GetRandomMessage();
            
            // Var respond to user
            var response = new DiscordInteractionResponseBuilder().WithContent(eightBallMessage);
            await ctx.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in message handler");
        }
    }

    private string GetRandomMessage()
    {
        if (_random.Next(1000) == 0)
        {
            return "DEAR GOD NO!";
        }

        var answerIdx = _random.Next(_options.EightBallResponses.Length);
        return _options.EightBallResponses[answerIdx];
    }
}

public class M8CommandOptions
{
    [Required]
    public string[] EightBallResponses { get; init; } = Array.Empty<string>();
}