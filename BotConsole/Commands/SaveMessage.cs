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

        [Command("Save Message")]
        [CommandType(ApplicationCommandType.Message)]
        [Ephemeral]
        public async Task<IResult> SaveMessageAction(IMessage message)
        {
            var model = SavedMessageModel.FromMessage(message, _interactionContext);

            var isDupe = await _dbContext.SavedMessages.Where(x => x.InvokerID == model.InvokerID && x.GuildID == model.GuildID && x.MessageID == model.MessageID).AnyAsync();
            if (!isDupe)
            {
                await _dbContext.SavedMessages.AddAsync(model, this.CancellationToken);
                await _dbContext.SaveChangesAsync();
                return await _feedbackService.SendContextualSuccessAsync($"[Message]({model.MessageUrl}) was saved to your list!", ct: this.CancellationToken);
            }
            return await _feedbackService.SendContextualErrorAsync($"[Message]({model.MessageUrl}) was already in your list!", ct: this.CancellationToken);
        }

        [Command("retrieve")]
        [Description("Retrieves your saved messages")]
        [CommandType(ApplicationCommandType.ChatInput)]
        [Ephemeral]
        public async Task<IResult> RetrieveSavedMessages()
        {
            _interactionContext.TryGetUserID(out Snowflake? userID);

            return await RetrieveSavedMessagesOf(new PartialUser() { ID = userID.AsOptional() });
            //var messages = await _dbContext.SavedMessages.Where(x => x.InvokerID == userID.ToString()).ToListAsync();
            //StringBuilder sb = new();
            //int i = 0;
            //foreach (var msg in messages)
            //{
            //    sb.AppendLine($"{i + 1}: [THIS]({msg.MessageUrl}) by <@{msg.AuthorID}> in <#{msg.ChannelID}>");
            //    i++;
            //}

            //Embed embed = new($"You have {messages.Count} saved messages!", Description: sb.ToString());

            //return await _feedbackService.SendContextualEmbedAsync(embed, ct: this.CancellationToken);
        }

        [Command("spy")]
        [Description("Retrieves your saved messages")]
        [CommandType(ApplicationCommandType.ChatInput)]
        [DiscordDefaultMemberPermissions(DiscordPermission.Administrator)]
        [Ephemeral]
        public async Task<IResult> RetrieveSavedMessagesOf([Description("The user of which to retrieve the messages")] IPartialUser user)
        {
            var messages = await _dbContext.SavedMessages.Where(x => x.InvokerID == user.ID.ToString()).ToListAsync();
            StringBuilder sb = new();
            int i = 0;
            foreach (var msg in messages)
            {
                sb.AppendLine($"{i + 1}: [THIS]({msg.MessageUrl}) by <@{msg.AuthorID}> in <#{msg.ChannelID}>");
                i++;
            }

            Embed embed = new($"{user.Username} has {messages.Count} saved messages!", Description: sb.ToString());

            return await _feedbackService.SendContextualEmbedAsync(embed, ct: this.CancellationToken);
        }

        [Command("drop")]
        [Description("Deletes your saved messages (ALL OF THEM ATM)")]
        [CommandType(ApplicationCommandType.ChatInput)]
        [Ephemeral]
        public async Task<IResult> DropAllMessages()
        {
            _interactionContext.TryGetUserID(out Snowflake? userID);
            var messages = await _dbContext.SavedMessages.Where(x => x.InvokerID == userID.ToString()).ToListAsync();
            _dbContext.SavedMessages.RemoveRange(messages);
            var number = await _dbContext.SaveChangesAsync();
            return await _feedbackService.SendContextualSuccessAsync($"Successfully deleted {number} messages!");
        }
    }
}
