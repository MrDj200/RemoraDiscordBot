using BotConsole.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Rest.Core;
using Remora.Results;
using System.ComponentModel;
using System.Text;

namespace BotConsole.Commands
{
    internal class SaveMessage : CommandGroup
    {
        private readonly FeedbackService _feedbackService;
        private readonly ILogger<MessageStats> _logger;
        private readonly ICommandContext _commandContext;
        private readonly BotDBContext _dbContext;
        private readonly IInteractionContext _interactionContext;

        public SaveMessage(FeedbackService feedbackService, ILogger<MessageStats> logger, ICommandContext context, BotDBContext dBContext, IInteractionContext interactionContext)
        {
            _feedbackService = feedbackService;
            _logger = logger;
            _commandContext = context;
            _dbContext = dBContext;
            _interactionContext = interactionContext;
        }

        // TODO: Button on messages to save image to database (possibly to disk) and then command to retrieve saved stuff

        [Command("Save Message")]
        [CommandType(ApplicationCommandType.Message)]
        [Ephemeral]
        public async Task<IResult> SaveMessageAction(IMessage message)
        {
            _logger.LogDebug("STARTING SAVE MESSAGE STUFF!###################################");
            var model = SavedMessageModel.FromMessage(message, _interactionContext);
            await _dbContext.SavedMessages.AddAsync(model, this.CancellationToken);
            await _dbContext.SaveChangesAsync();
            _logger.LogDebug("FINISHED SAVE MESSAGE STUFF!###################################");

            return await _feedbackService.SendContextualAsync("Ahhhhhh", ct: this.CancellationToken);
        }

        [Command("retrieve")]
        [Description("Retrieves your saved messages")]
        [CommandType(ApplicationCommandType.ChatInput)]
        [Ephemeral]
        public async Task<IResult> RetrieveSavedMessages()
        {
            _interactionContext.TryGetUserID(out Snowflake? userID);
            var messages = await _dbContext.SavedMessages.Where(x => x.InvokerID == userID.ToString()).ToListAsync();
            StringBuilder sb = new();
            int i = 0;
            foreach (var msg in messages)
            {
                sb.AppendLine($"[{i + 1}]({msg.MessageUrl})");
                i++;
            }

            Embed embed = new($"You ( <@{userID}> ) have {messages.Count} saved messages!", Description: sb.ToString());

            return await _feedbackService.SendContextualEmbedAsync(embed, ct: this.CancellationToken);
        }
    }
}
