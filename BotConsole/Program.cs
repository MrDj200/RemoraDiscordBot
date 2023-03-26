using BotConsole.Commands;
using BotConsole.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Remora.Commands.Extensions;
using Remora.Discord.API.Abstractions.Gateway.Commands;
using Remora.Discord.Commands.Extensions;
using Remora.Discord.Gateway;
using Remora.Discord.Gateway.Extensions;

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

            var _botToken = args[0];
            if (string.IsNullOrEmpty(_botToken))
            {
                Console.WriteLine("NO BOT TOKEN GIVEN IN PARAMETER!");
                return;
            }

            IHost host = Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services
                        .AddDiscordGateway(_ => _botToken)
                        .AddResponder<TwitterResponder>()
                        .AddResponder<RockAndStoneResponder>()
                        .AddSingleton<DRGMessageProvider>()
                        .Configure<DiscordGatewayClientOptions>(g => g.Intents |= GatewayIntents.MessageContents | GatewayIntents.GuildPresences)
                        .AddSingleton<HttpClient>()
                        .AddLogging(loggingBuilder =>
                        {
                            loggingBuilder.AddConsole();
                        });

                    services.AddDiscordCommands(true).AddCommandTree()
                        .WithCommandGroup<HttpCatCommands>()
                        .WithCommandGroup<MessageStats>()
                        .WithCommandGroup<VRCXMetadataResolve>();

                    services
                        .AddHostedService<RemoraBotWorker>();

                }).Build();

            host.Run();
        }
    }

}