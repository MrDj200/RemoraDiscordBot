using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Remora.Discord.API.Abstractions.Gateway.Commands;
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

            var services = new ServiceCollection()
                .AddDiscordGateway(_ => botToken)
                .AddResponder<PingPongResponder>()
                .AddResponder<TwitterResponder>()
                .Configure<DiscordGatewayClientOptions>(g => g.Intents |= GatewayIntents.MessageContents | GatewayIntents.GuildPresences)
                .AddLogging(loggingBuilder =>
                {
                    loggingBuilder.AddConsole();
                })
                .BuildServiceProvider();

            var gatewayClient = services.GetRequiredService<DiscordGatewayClient>();

            var runResult = await gatewayClient.RunAsync(cancellationSource.Token);

            var log = services.GetRequiredService<ILogger<Program>>();

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