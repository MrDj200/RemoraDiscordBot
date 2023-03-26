using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BotConsole.Database
{
    public class BotDBContext : DbContext
    {
        private readonly static string _fallBackSqlite = "Data Source=DebuggingDB.db";
        public DbSet<SavedMessageModel> SavedMessages { get; set; }

        private static ILogger<BotDBContext>? _logger;

        public BotDBContext(DbContextOptions<BotDBContext> options, ILogger<BotDBContext> logger) : base(options)
        {
            _logger = logger;
            DoMigrationsAsync().Wait();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var args = Environment.GetCommandLineArgs();

#if DEBUG
            _logger?.LogWarning("Using SQLite! (DEBUG)");
            optionsBuilder.UseSqlite(_fallBackSqlite);
#else
            if (args.Length >= 2)
            {
                //var serverVersion = ServerVersion.AutoDetect(args[1]);
                var serverVersion = ServerVersion.Parse("8.0.22-mysql");
                if (serverVersion != null)
                {
                    _logger?.LogWarning("Using MySql!");
                    optionsBuilder.UseMySql(args[1], serverVersion);
                }
                else
                {
                    _logger?.LogWarning("Server Version is null! Using SQLite! (Fallback)");
                    optionsBuilder.UseSqlite(_fallBackSqlite);
                }
            }
            else
            {
                _logger?.LogWarning("Using SQLite! (Fallback)");
                optionsBuilder.UseSqlite(_fallBackSqlite);
            }
#endif
        }

        private async Task DoMigrationsAsync()
        {
            var pending = await this.Database.GetPendingMigrationsAsync();
            if (pending.Any())
            {
                _logger?.LogInformation("Applying {count} migrations", pending.Count());
                await this.Database.MigrateAsync();
                _logger?.LogInformation("Finished migrations!");
            }
        }
    }
}
