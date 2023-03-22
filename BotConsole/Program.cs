using BotConsole.Commands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remora.Commands.Extensions;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Commands.Services;
using Remora.Discord.Gateway;
using Remora.Discord.Gateway.Extensions;
using Remora.Discord.Gateway.Results;
using Remora.Results;

namespace BotConsole
{
    internal class Program
    {

        static void Main(string[] args)
        {

#if DEBUG
            Console.WriteLine("STARTING IN DEBUG MODE!!!!!!!!!");
#else
            Console.WriteLine("Starting in Release mode!");
#endif
            try
            {
                MainAsync(args).Wait();
            }
            catch (Exception e)
            {
                if (e is TaskCanceledException)
                {
                    Console.WriteLine("Task canceled");
                    return;
                }
                if (e is AggregateException && e.Message.Contains("canceled"))
                {
                    Console.WriteLine("Aggregate contains canceled");
                    return;
                }
                throw;
            }
            finally { Console.WriteLine("I'm outta here!"); }

        }

        static async Task MainAsync(string[] args)
        {
            Console.WriteLine("Starting bot!");

            var cancellationSource = new CancellationTokenSource();
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                Console.WriteLine("Ending program...");
                eventArgs.Cancel = true;
                cancellationSource.Cancel();
            };

            var botToken = args[0];
            if (botToken == null)
            {
                Console.WriteLine("NO BOT TOKEN GIVEN IN PARAMETER!");
                return;
            }

            var servicesRaw = new ServiceCollection()
                .AddDiscordGateway(_ => botToken)
                .AddResponder<TwitterResponder>()
                .AddResponder<RockAndStoneResponder>()
                .AddSingleton<DRGMessageProvider>()
                .Configure<DiscordGatewayClientOptions>(g => g.Intents |= GatewayIntents.MessageContents | GatewayIntents.GuildPresences)
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.AddConsole();
                });

            servicesRaw.AddDiscordCommands(true).AddCommandTree()
                .WithCommandGroup<HttpCatCommands>()
                .WithCommandGroup<MessageStats>();


            var services = servicesRaw.BuildServiceProvider();
            var log = services.GetRequiredService<ILogger<Program>>();

        #region SlashCommandStuff?
            var slashService = services.GetRequiredService<SlashService>();
            var updateSlash = await slashService.UpdateSlashCommandsAsync(ct: cancellationSource.Token);
            if (!updateSlash.IsSuccess)
            {
                log.LogWarning("Failed to update slash commands: {Reason}", updateSlash.Error.Message);
            }
        #endregion

            var gatewayClient = services.GetRequiredService<DiscordGatewayClient>();
            var runResult = await gatewayClient.RunAsync(cancellationSource.Token);                    

            if (!runResult.IsSuccess)
            {
                switch (runResult.Error)
                {
                    case ExceptionError exe:
                        {
                            log.LogError
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
                            log.LogError("Gateway error: {Message}", runResult.Error.Message);
                            break;
                        }
                    default:
                        {
                            log.LogError("Unknown error: {Message}", runResult.Error.Message);
                            break;
                        }
                }
            }

            await Task.Delay(-1, cancellationSource.Token);
        }
    }

}