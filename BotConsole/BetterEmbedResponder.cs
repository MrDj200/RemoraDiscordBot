using Microsoft.Extensions.Logging;
using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Gateway.Responders;
using Remora.Results;
using System.Text.RegularExpressions;

namespace BotConsole
{
    public class BetterEmbedResponder : IResponder<IMessageCreate>
    {
        private readonly IDiscordRestChannelAPI _channelAPI;
        private readonly ILogger<BetterEmbedResponder> _logger;

        private readonly Regex _twitterRegex = new Regex(@"https?:\/\/(?:www\.)?(x|twitter)\.com\/\w+\/status\/\d+(\/photos\/\d)?", RegexOptions.Compiled);
        private readonly Regex _tiktokRegex = new Regex(@"https?:\/\/(?:www\.)?tiktok.com\/(@\w+\/video\/\d+|t\/\w+)", RegexOptions.Compiled);
        private readonly Regex _redditRegex = new Regex(@"https?:\/\/(?:www\.)?reddit.com\/r\/\w+\/[^\s]+", RegexOptions.Compiled);

        public BetterEmbedResponder(IDiscordRestChannelAPI channelAPI, ILogger<BetterEmbedResponder> logger)
        {
            _channelAPI = channelAPI;
            _logger = logger;
        }

        public async Task<Result> RespondAsync(IMessageCreate gatewayEvent, CancellationToken ct = default)
        {
            if (gatewayEvent.Author.IsBot is { HasValue: true, Value: true } || gatewayEvent.WebhookID.HasValue)
            {
                return Result.FromSuccess();
            }

            var twitterMatch = _twitterRegex.Match(gatewayEvent.Content);
            if (twitterMatch.Success)
            {
                await Task.Delay(1000, ct); // Wait a second for slow twitter to get an embed

                await _channelAPI.EditMessageAsync(gatewayEvent.ChannelID, gatewayEvent.ID, embeds: null, flags: MessageFlags.SuppressEmbeds); // Remove the original embed

                _logger.LogInformation($"Converting twitter link {twitterMatch.Value}");

                string replacedText = twitterMatch.Value.Replace("twitter.com", "vxtwitter.com");
                replacedText = replacedText.Replace("x.com", "vxtwitter.com");

                return (Result)await _channelAPI.CreateMessageAsync
                (
                    gatewayEvent.ChannelID,
                    content: $"It's dangerous to go alone! Here, take this (better) embed: {replacedText}",
                    ct: ct
                );
            }
            var redditMatch = _redditRegex.Match(gatewayEvent.Content);
            if (redditMatch.Success)
            {
                await Task.Delay(1000, ct); // Wait a second for slow tiktok to get an embed

                await _channelAPI.EditMessageAsync(gatewayEvent.ChannelID, gatewayEvent.ID, embeds: null, flags: MessageFlags.SuppressEmbeds); // Remove the original embed

                _logger.LogInformation($"Converting reddit link {redditMatch.Value}");

                return (Result)await _channelAPI.CreateMessageAsync
                (
                    gatewayEvent.ChannelID,
                    content: $"It's dangerous to go alone! Here, take this (better) embed: {redditMatch.Value.Replace("reddit.com", "rxddit.com")}",
                    ct: ct
                );
            }
            var tiktokMatch = _tiktokRegex.Match(gatewayEvent.Content);
            if (tiktokMatch.Success)
            {
                await Task.Delay(1000, ct); // Wait a second for slow tiktok to get an embed

                await _channelAPI.EditMessageAsync(gatewayEvent.ChannelID, gatewayEvent.ID, embeds: null, flags: MessageFlags.SuppressEmbeds); // Remove the original embed

                _logger.LogInformation($"Converting tiktok link {tiktokMatch.Value}");

                return (Result)await _channelAPI.CreateMessageAsync
                (
                    gatewayEvent.ChannelID,
                    content: $"It's dangerous to go alone! Here, take this (better) embed: {tiktokMatch.Value.Replace("tiktok.com", "vxtiktok.com")}",
                    ct: ct
                );
            }

            return Result.FromSuccess();

        }
    }

}