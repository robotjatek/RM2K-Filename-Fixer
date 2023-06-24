using Microsoft.Extensions.DependencyInjection;

using RMFileNameFixer.Model;
using RMFileNameFixer.Service;
using RMFileNameFixer.Service.Interface;
using Serilog;

using System.Text.Json;

namespace RMFileNameFixer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = SetupIOC();
            var mainService = serviceProvider.GetRequiredService<IMainService>();

            if (args.Length != 0 && args[0].ToUpperInvariant().Equals("restore", StringComparison.InvariantCultureIgnoreCase))
            {
                mainService.RestoreBackup();
                return;
            }

            mainService.InteractUser();
        }

        private static ServiceProvider SetupIOC()
        {
            var options = JsonSerializer.Deserialize<RmFixerOptions>(File.ReadAllText("rm_fixer_options.json")) ?? throw new Exception();

            var logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File("log.txt")
                .CreateLogger();

            return new ServiceCollection()
                .AddSingleton(options)
                .AddSingleton<ILogger>(logger)
                .AddSingleton<ISystemWrapper, SystemWrapper>()
                .AddSingleton<IMainService, MainService>()
                .AddSingleton<IFilenameService, FilenameService>()
                .AddSingleton<IRenameService, RenameService>()
                .AddSingleton<IBackupService, BackupService>()
                .BuildServiceProvider();
        }
    }
}