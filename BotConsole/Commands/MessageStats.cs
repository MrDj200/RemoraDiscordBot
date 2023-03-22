using Microsoft.Extensions.Logging;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Abstractions.Rest;
using Remora.Discord.Commands.Feedback.Messages;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Rest.Core;
using Remora.Results;
using System.ComponentModel;
using System.Drawing;
using System.Text;

namespace BotConsole.Commands
{
    internal class MessageStats : CommandGroup
    {
        private readonly FeedbackService _feedbackService;
        private readonly ILogger<MessageStats> _logger;
        private readonly IDiscordRestChannelAPI _channelAPI;
        public MessageStats(FeedbackService feedbackService, ILogger<MessageStats> logger, IDiscordRestChannelAPI channelAPI)
        {
            _feedbackService = feedbackService;
            _logger = logger;
            _channelAPI = channelAPI;
        }

        [Command("channel-stats")]
        [Description("Prints out stats for the given channel")]
        public async Task<IResult> ChannelStats([Description("The channel for which to get the stats")] IChannel channel)
        {            
            var messagesResult = await GetAllChannelMessagesAsync(channel.ID, ct: this.CancellationToken);
            if (!messagesResult.IsSuccess)
            {
                return await _feedbackService.SendContextualErrorAsync($"Something went wrong with your request!: \n{messagesResult.Error.Message}");
            }
            var messages = messagesResult.Entity;
            var authorGroups = messages.GroupBy(x => x.Author).OrderByDescending(g => g.Count());

            StringBuilder sb = new();

            sb.AppendLine($"Total messages in <#{channel.ID}>: {messages.Count()}");
            foreach (var authorGroup in authorGroups)
            {
                sb.Append($"<@{authorGroup.Key.ID}>: {authorGroup.Count()}");
                if (authorGroup.Key.IsBot is { HasValue: true, Value: true })
                {
                    sb.Append("\t|\tBOT");
                }
                sb.AppendLine();
            }

            return (Result)await _feedbackService.SendContextualMessageAsync(new FeedbackMessage(sb.ToString(), Color.AliceBlue));
        }

        /// <summary>
        /// Gets all the messages for a channel.
        /// </summary>
        /// <param name="channelID">The ID of the channel.</param>
        /// <param name="ct">The cancellation token for this operation.</param>
        /// <returns>A retrieval result which may or may not have succeeded.</returns>
        async Task<Result<IReadOnlyList<IMessage>>> GetAllChannelMessagesAsync(Snowflake channelID, CancellationToken ct = default)
        {            
            var result = await _channelAPI.GetChannelMessagesAsync(channelID, limit: 100, ct: ct);
            if (!result.IsSuccess)
            {
                return result;
            }
            var messages = result.Entity;
            List<IMessage> workList = new(messages);
            bool done = messages.Count < 100;
            int i = 1;
            while (!done)
            {
                if (i >= 35)
                {
                    await Task.Delay(1000, ct); // Mitigating being rate limited
                }
                var result2 = await _channelAPI.GetChannelMessagesAsync(channelID, limit: 100, ct: ct, before: workList.Last().ID);
                if (!result2.IsSuccess)
                {
                    return result2;
                }
                done = result2.Entity.Count < 100;
                workList.AddRange(result2.Entity);
            }

            return workList.AsReadOnly();
        }
    }
}
