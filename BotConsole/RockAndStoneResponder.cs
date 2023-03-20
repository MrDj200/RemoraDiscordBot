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

        static string nitradescription = @"<:nitra:406463687923073024><:nitra:406463687923073024><:nitra:406463687923073024><:nitra:406463687923073024><:nitra:406463687923073024><:nitra:406463687923073024><:nitra:406463687923073024><:nitra:406463687923073024>
THERE'S NITRA OVA HERE
<:nitra:406463687923073024><:nitra:406463687923073024><:nitra:406463687923073024><:nitra:406463687923073024><:nitra:406463687923073024><:nitra:406463687923073024><:nitra:406463687923073024><:nitra:406463687923073024>";

        private readonly IDiscordRestChannelAPI _channelAPI;
        private readonly DRGMessageProvider _drg;
        public RockAndStoneResponder(IDiscordRestChannelAPI channelAPI, DRGMessageProvider drg)
        {
            _channelAPI = channelAPI;
            _drg = drg;
        }

        public async Task<Result> RespondAsync(IMessageCreate gatewayEvent, CancellationToken ct = default)
        {
            if (gatewayEvent.Author.IsBot.HasValue && gatewayEvent.Author.IsBot.Value || gatewayEvent.WebhookID.HasValue)
            {
                return Result.FromSuccess();
            }
            Embed embed;
            if (gatewayEvent.Content.ToLower().Contains("rock and stone"))
            {
                embed = new Embed(Description: _drg.RandomRockResponse(), Colour: Color.Purple);
            }
            else if (gatewayEvent.Content.ToLower().Contains("nitra"))
            {
                embed = new Embed
                (
                    Description: _drg.RandomNitraResponse(),
                    Colour: Color.DarkRed
                );
            }
            else if (gatewayEvent.Content.ToLower().Contains("we're rich") || gatewayEvent.Content.ToLower().Contains("we are rich"))
            {
                embed = new Embed
                (
                    Description: "We're Rich!",
                    Colour: Color.Gold
                );
            }
            else if (gatewayEvent.Content.ToLower().Contains("stone"))
            {
                embed = new Embed
                (
                    Description: _drg.RandomStoneResponse(),
                    Colour: Color.Gray
                );
            }
            else if (gatewayEvent.Content.ToLower().Contains("mushroom"))
            {
                embed = new Embed
                (
                    Description: "Mushroom!",
                    Colour: Color.Brown
                );
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
