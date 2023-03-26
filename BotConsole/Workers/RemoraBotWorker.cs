using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remora.Discord.Commands.Services;
using Remora.Discord.Gateway;
using Remora.Discord.Gateway.Results;
using Remora.Results;

namespace BotConsole.Workers
{
    internal class RemoraBotWorker : BackgroundService
    {
        private readonly ILogger<RemoraBotWorker> _logger;
        private readonly SlashService _slashService;
        private readonly DiscordGatewayClient _gatewayClient;

        public RemoraBotWorker(ILogger<RemoraBotWorker> logger, SlashService slashService, DiscordGatewayClient gatewayClient)
        {
            _logger = logger;
            _slashService = slashService;
            _gatewayClient = gatewayClient;
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("TRYING TO GRACEFULLY SHUTDOWN!");
            _gatewayClient.Dispose();
            return Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Starting bot!");

            var updateSlash = await _slashService.UpdateSlashCommandsAsync(ct: stoppingToken);
            if (!updateSlash.IsSuccess)
            {
                _logger.LogWarning("Failed to update slash commands: {Reason}", updateSlash.Error.Message);
            }

            var runResult = await _gatewayClient.RunAsync(stoppingToken);

            if (!runResult.IsSuccess)
            {
                switch (runResult.Error)
                {
                    case ExceptionError exe:
                        {
                            _logger.LogError
                            (
                                exe.Exception,
                                "Exception during gateway connection: {ExceptionMessage}",
                                exe.Message
                            );

                            break;
                        }
                    case GatewayWebSocketError:
                    case GatewayDiscordError:
                        {
                            _logger.LogError("Gateway error: {Message}", runResult.Error.Message);
                            break;
                        }
                    default:
                        {
                            _logger.LogError("Unknown error: {Message}", runResult.Error.Message);
                            break;
                        }
                }
            }
        }
    }
}
