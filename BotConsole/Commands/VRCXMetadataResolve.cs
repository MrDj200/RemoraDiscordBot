using BotConsole.Models;
using Microsoft.Extensions.Logging;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Rest.Core;
using Remora.Results;
using System.Text;
using System.Text.Json;

namespace BotConsole.Commands
{
    internal class VRCXMetadataResolve : CommandGroup
    {
        private readonly FeedbackService _feedbackService;
        private readonly ILogger<VRCXMetadataResolve> _logger;
        private readonly HttpClient _httpClient;
        private readonly ICommandContext _context;

        public VRCXMetadataResolve(FeedbackService feedbackService, ILogger<VRCXMetadataResolve> logger, HttpClient httpClient, ICommandContext context)
        {
            _feedbackService = feedbackService;
            _logger = logger;
            _httpClient = httpClient;
            _context = context;
        }

        [Command("Get World")]
        [CommandType(ApplicationCommandType.Message)]
        public async Task<IResult> GetWorldFromMetadata(IMessage message)
        {
            var attachments = message.Attachments;
            if (attachments == null || attachments.Count == 0)
            {
                return (Result)await _feedbackService.SendContextualErrorAsync("No attachments found!", ct: this.CancellationToken);
            }

            IEnumerable<Task<VRCWorld?>> tasks = attachments.Select(x => GetWorldFromImageUrl(x.Url));
            Task.WaitAll(tasks.ToArray());

            var worldsWithLinks = tasks.Where(x => x.Result != null).GroupBy(x => x.Result?.id).Select(x => $"[{x.First().Result?.name}](https://vrchat.com/home/world/{x.First().Result?.id})");

            string messageUrl = "https://www.youtube.com/watch?v=dQw4w9WgXcQ";
            if (_context.TryGetGuildID(out Snowflake? GuildID))
            {
                messageUrl = $"https://discord.com/channels/{GuildID}/{message.ChannelID}/{message.ID}";
            }
            string FinalMessage = $"Found {attachments.Count} attachments on [this message]({messageUrl}):\n{string.Join("\n", worldsWithLinks)}";

            if (!worldsWithLinks.Any())
            {
                FinalMessage = $"Found {attachments.Count} attachments on [this message]({messageUrl}), \nbut none of them had any valid MetaData. \nAsk <@165129990424231936> for more info on this!";
            }


            return (Result)await _feedbackService.SendContextualInfoAsync(FinalMessage);
        }

        private async Task<VRCWorld?> GetWorldFromImageUrl(string url)
        {
            if (await Utils.IsUrlPngAsync(url))
            {
                string pngChunkResult;
                var response = await _httpClient.GetByteArrayAsync(url);
                pngChunkResult = await Utils.ReadPngChunkArray(response);

                if (pngChunkResult != null && pngChunkResult != "N/A")
                {
                    var meta = await JsonSerializer.DeserializeAsync<VRCXMeta>(new MemoryStream(Encoding.UTF8.GetBytes(pngChunkResult)), cancellationToken: this.CancellationToken);

                    return meta?.world;
                }
            }
            else
            {
                _logger.LogWarning("{url} was not a png", url);
            }
            return null;
        }

    }
}
