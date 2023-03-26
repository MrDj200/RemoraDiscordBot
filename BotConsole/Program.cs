using BotConsole.Commands;
using BotConsole.Database;
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
        public static bool ISDEBUG { get; private set; }

        static void Main(string[] args)
        {

#if DEBUG
            Console.WriteLine("STARTING IN DEBUG MODE!!!!!!!!!");
            ISDEBUG = true;
#else
            Console.WriteLine("Starting in Release mode!");
            ISDEBUG = false;
#endif

            var _botToken = args[0];
            if (string.IsNullOrEmpty(_botToken))
            {
                Console.WriteLine("NO BOT TOKEN GIVEN IN PARAMETER!");
                return;
            }
            if (!ISDEBUG)
            {
                if (args.Length < 2 || string.IsNullOrEmpty(args[1]))
                {
                    Console.WriteLine("NO DATABASE CONNECTION STRING! Falling back to sqlite!");
                    Console.WriteLine("Current args: ");
                    foreach (var item in args)
                    {
                        Console.WriteLine(item);
                    }
                }                
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
                        .WithCommandGroup<SaveMessage>()
                        .WithCommandGroup<VRCXMetadataResolve>();

                    services
                        .AddHostedService<RemoraBotWorker>();

                    services
                        .AddDbContext<BotDBContext>();

                }).Build();

            host.Run();
        }
    }

}