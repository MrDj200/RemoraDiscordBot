using Microsoft.Extensions.Logging;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Gateway.Responders;
using Remora.Results;
using System.Text.RegularExpressions;

namespace BotConsole
{
    public class TwitterResponder : IResponder<IMessageCreate>
    {
        private readonly IDiscordRestChannelAPI _channelAPI;
        private readonly ILogger<TwitterResponder> _logger;

        public TwitterResponder(IDiscordRestChannelAPI channelAPI, ILogger<TwitterResponder> logger)
        {
            _channelAPI = channelAPI;
            _logger = logger;
        }

        public async Task<Result> RespondAsync(IMessageCreate gatewayEvent, CancellationToken ct = default)
        {
            if (gatewayEvent.Author.IsBot.HasValue && gatewayEvent.Author.IsBot.Value || gatewayEvent.WebhookID.HasValue)
            {
                return Result.FromSuccess();
            }

            var match = Regex.Match(gatewayEvent.Content, @"https?:\/\/twitter\.com\/\w+\/status\/\d+(\/photos\/\d)?");

            if (!match.Success)
            {
                return Result.FromSuccess();
            }

            await Task.Delay(1000, ct); // Wait a second for slow twitter to get an embed

            await _channelAPI.EditMessageAsync(gatewayEvent.ChannelID, gatewayEvent.ID, embeds: null, flags: MessageFlags.SuppressEmbeds); // Remove the original embed

            _logger.LogInformation($"Converting twitter link {match.Value}");

            return (Result)await _channelAPI.CreateMessageAsync
            (
                gatewayEvent.ChannelID,
                content: $"It's dangerous to go alone! Here, take this (better) embed: {match.Value.Replace("twitter.com", "fxtwitter.com")}",
                ct: ct
            );
        }
    }

}