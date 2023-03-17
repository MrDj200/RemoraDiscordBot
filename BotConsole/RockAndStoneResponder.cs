using Remora.Discord.API.Abstractions.Gateway.Events;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.API.Objects;
using Remora.Discord.Gateway.Responders;
using Remora.Results;
using System.Drawing;

namespace BotConsole
{
    internal class RockAndStoneResponder : IResponder<IMessageCreate>
    {
        private readonly IDiscordRestChannelAPI _channelAPI;
        public RockAndStoneResponder(IDiscordRestChannelAPI channelAPI)
        {
            _channelAPI = channelAPI;
        }

        public async Task<Result> RespondAsync(IMessageCreate gatewayEvent, CancellationToken ct = default)
        {
            Embed embed;

            if (gatewayEvent.Author.IsBot.HasValue && gatewayEvent.Author.IsBot.Value || gatewayEvent.WebhookID.HasValue)
            {
                return Result.FromSuccess();
            }

            if (gatewayEvent.Content.ToLower().Contains("rock and stone"))
            {
                embed = new Embed(Description: "FOR ROCK AND STONE!", Colour: Color.Purple);
            }
            else if (gatewayEvent.Content.ToLower().Contains("nitra"))
            {
                embed = new Embed(Description: "THERE'S NITRA OVER HERE!", Colour: Color.DarkRed);
            }
            else
            {
                return Result.FromSuccess();
            }

            return (Result)await _channelAPI.CreateMessageAsync
            (
            gatewayEvent.ChannelID,
                embeds: new[] { embed },
                ct: ct
            );
        }
    }
}
