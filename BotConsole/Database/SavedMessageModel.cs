using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.Commands.Contexts;
using Remora.Discord.Commands.Extensions;
using Remora.Rest.Core;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BotConsole.Database
{
    public class SavedMessageModel
    {
        [Key]
        public Guid ID { get; set; }
        public string AuthorID { get; set; }
        public string InvokerID { get; set; }
        public string? GuildID { get; set; }
        public string ChannelID { get; set; }
        public string MessageID { get; set; }

        [NotMapped]
        public string MessageUrl => $"https://discord.com/channels/{GuildID}/{ChannelID}/{MessageID}";

        public static SavedMessageModel FromMessage(IMessage message, IInteractionContext interactionContext)
        {
            interactionContext.TryGetGuildID(out Snowflake? guildID);
            if (!interactionContext.TryGetUserID(out Snowflake? invokerID) || invokerID == null)
            {
                throw new Exception("Could not get InvokerID in a command. Doesn't make sense");
            }

            return new SavedMessageModel
            {
                AuthorID = message.Author.ID.ToString(),
                InvokerID = invokerID.ToString(),
                GuildID = guildID.ToString(),
                ChannelID = message.ChannelID.ToString(),
                MessageID = message.ID.ToString()
            };
        }

    }
}
