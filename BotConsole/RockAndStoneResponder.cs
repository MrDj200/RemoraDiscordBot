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
            string msg = gatewayEvent.Content.ToLower();
            if (msg.Contains("rock and stone"))
            {
                embed = new Embed
                (
                    Title: _drg.RockyResponses.RandomEntry(),
                    Colour: Color.Purple
                );
            }
            else if (msg.Contains("nitra"))
            {
                embed = new Embed
                (
                    Title: _drg.NitraResponses.RandomEntry(),
                    Colour: Color.DarkRed
                );
            }
            else if (msg.Contains("we're rich", "we are rich"))
            {
                embed = new Embed
                (
                    Title: "We're Rich!",
                    Colour: Color.Gold,
                    Thumbnail: new EmbedThumbnail("https://cdn.discordapp.com/emojis/587343470977351695.webp?size=1024&quality=lossless")
                );
            }
            else if (msg.Contains("stone"))
            {
                embed = new Embed
                (
                    Title: _drg.StoneResponses.RandomEntry(),
                    Colour: Color.Gray
                );
            }
            else if (msg.Contains("mushroom"))
            {
                embed = new Embed
                (
                    Author: new EmbedAuthor("Mission Control", IconUrl: "https://deeprockgalactic.wiki.gg/images/d/d2/Mission_control_portrait.png"),
                    Title: "Mushroom!",
                    Colour: Color.Brown
                );
            }
            else if (msg.Contains("goo sack"))
            {
                embed = new Embed
                (
                    Title: _drg.GooSackResponses.RandomEntry(),
                    Colour: Color.Yellow
                );
            }
            else if (msg.Contains("ebonut"))
            {
                embed = new Embed
                (
                    Title: _drg.EbonutResponses.RandomEntry(),
                    Colour: Color.SandyBrown
                );
            }
            else
            {
                return Result.FromSuccess();
            }

            embed = AddRandomShit(embed, 10);

            return (Result)await _channelAPI.CreateMessageAsync
            (
                gatewayEvent.ChannelID,
                embeds: new[] { embed },
                ct: ct
            );
        }

        Embed AddRandomShit(Embed embed, int percentage = 10)
        {
            var randomVal = new Random().Next(100);
            Embed newEmbed;
            if (randomVal >= 100 - percentage)
            {
                newEmbed = new Embed
                (
                    Title: embed.Title,
                    Type: embed.Type,
                    Description: embed.Description,
                    Timestamp: embed.Timestamp,
                    Colour: embed.Colour,
                    Footer: embed.Footer,
                    Image: embed.Image,
                    Thumbnail: embed.Thumbnail,
                    Video: embed.Video,
                    Provider: embed.Provider,
                    Author: embed.Author,
                    Fields: embed.Fields,

                    Url: "https://www.youtube.com/watch?v=dQw4w9WgXcQ" // TODO: Add more random shit. Don't alway rickroll
                );

                return newEmbed;
            }

            return embed;
        }

    }
}
