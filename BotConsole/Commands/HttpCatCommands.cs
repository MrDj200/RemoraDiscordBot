using Microsoft.Extensions.Logging;
using Remora.Commands.Attributes;
using Remora.Commands.Groups;
using Remora.Discord.API.Abstractions.Objects;
using Remora.Discord.API.Objects;
using Remora.Discord.Commands.Attributes;
using Remora.Discord.Commands.Feedback.Services;
using Remora.Results;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using Remora.Discord.Commands.Feedback.Messages;
using Remora.Discord.Commands.Services;
using Remora.Discord.Commands.Extensions;
using Remora.Rest.Core;

namespace BotConsole.Commands
{
    internal class HttpCatCommands : CommandGroup
    {
        private readonly FeedbackService _feedbackService;
        private readonly ILogger<HttpCatCommands> _logger;
        private readonly ContextInjectionService _contextInjection;

        public HttpCatCommands(FeedbackService feedbackService, ILogger<HttpCatCommands> logger, ContextInjectionService contextInjection)
        {
            _feedbackService = feedbackService;
            _logger = logger;
            _contextInjection = contextInjection;
        }

        //[Command("Cattify")]
        //[CommandType(ApplicationCommandType.User)]
        //public async Task<IResult> PostContextualUserHttpCatAsync(IUser user)
        //{
        //    return await PostUserHttpCatAsync(user);
        //}

        [Command("MsgTest")]
        [CommandType(ApplicationCommandType.Message)]
        [Ephemeral]
        public async Task<IResult> MsgTest(IMessage message)
        {
            var context = _contextInjection.Context;
            string ugh;
            Snowflake? ChannelID, UserID, GuildID;
            if (context != null && context.TryGetChannelID(out ChannelID) && context.TryGetGuildID(out GuildID) && context.TryGetUserID(out UserID))
            {
                ugh = $"DEBUG\nUser: <@{UserID.Value}>\nGuild: {GuildID}\nChannel: <#{ChannelID}>\nTarget: {message.ID}";
                _logger.LogDebug("Yes");
            }
            else
            {
                ugh = "Didn't work. This is so sad";
                _logger.LogDebug("No");
            }
            return (Result)await _feedbackService.SendContextualMessageAsync(new FeedbackMessage(ugh, Color.Green));
        }

        [Command("Ping")]
        [CommandType(ApplicationCommandType.User)]
        public async Task<IResult> Ping(IUser user)
        {
            //var msg = new FeedbackMessage($"Hey <@{user.ID}>, you have been clicked!", (Color)user.AccentColour.Value);
            var opt = new FeedbackMessageOptions(IsTTS: false, AllowedMentions: new AllowedMentions(Users: new[] {user.ID}));
            //return (Result)await _feedbackService.SendContextualMessageAsync(msg, options: opt);
            var context = _contextInjection.Context;
            string ugh;
            Snowflake? ChannelID, UserID, GuildID;
            if (context != null && context.TryGetChannelID(out ChannelID) && context.TryGetGuildID(out GuildID) && context.TryGetUserID(out UserID))
            {
                ugh = $"DEBUG\nUser: <@{UserID.Value}>\nGuild: {GuildID}\nChannel: <#{ChannelID}>\nTarget: <@{user.ID}>";
                _logger.LogDebug("Yes");
            }
            else
            {
                ugh = "Didn't work. This is so sad";
                _logger.LogDebug("No");
            }



            return (Result)await _feedbackService.SendContextualMessageAsync(new FeedbackMessage(ugh, _feedbackService.Theme.FaultOrDanger), options: opt);
        }

#if DEBUG
        [Command("cat")]
        [Description("Posts a cat image that represents the given error code.")]
        public async Task<IResult> PostHttpCatAsync([Description("The HTTP code.")] int httpCode)
        {
            var embedImage = new EmbedImage($"https://http.cat/{httpCode}");
            var embed = new Embed(Colour: _feedbackService.Theme.Secondary, Image: embedImage);

            return (Result)await _feedbackService.SendContextualEmbedAsync(embed, ct: this.CancellationToken);
        }

        [Command("ephemeral-cat")]
        [Description("Posts a cat image that represents the given error code.")]
        [Ephemeral]
        public async Task<IResult> PostEphemeralHttpCatAsync([Description("The HTTP code.")] int httpCode)
        {
            var embedImage = new EmbedImage($"https://http.cat/{httpCode}");
            var embed = new Embed(Colour: _feedbackService.Theme.Secondary, Image: embedImage);
            
            return (Result)await _feedbackService.SendContextualEmbedAsync(embed, ct: this.CancellationToken);
        }

        [Command("user-cat")]
        [Description("Posts a cat image that matches the user.")]
        public Task<IResult> PostUserHttpCatAsync([Description("The user to cattify")] IPartialUser catUser)
        {
            if (!catUser.ID.TryGet(out var id))
            {
                return Task.FromResult<IResult>(Result.FromSuccess());
            }

            _logger.LogDebug($"Trying to cattify {catUser.Username} | Mail: {catUser.Email}");

            var values = Enum.GetValues<HttpStatusCode>();
            var index = Map(id.Value, 0, ulong.MaxValue, 0, (ulong)(values.Length - 1));

            _logger.LogDebug("Got Error code {code}", index);

            var code = values[index];
            return PostHttpCatAsync((int)code);
        }

        public Task<IResult> PostChannelHttpCatAsync([Description("The channel to cattify")][ChannelTypes(ChannelType.GuildText)] IChannel channel)
        {
            var values = Enum.GetValues<HttpStatusCode>();
            var index = Map(channel.ID.Value, 0, ulong.MaxValue, 0, (ulong)(values.Length - 1));

            var code = values[index];
            return PostHttpCatAsync((int)code);
        }


        private static ulong Map(ulong value, ulong fromSource, ulong toSource, ulong fromTarget, ulong toTarget)
        {
            return ((value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget)) + fromTarget;
        }
#endif
    }
}
