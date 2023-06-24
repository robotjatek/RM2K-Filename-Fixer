using RMFileNameFixer.Model;
using RMFileNameFixer.Service.Interface;

using Serilog;

namespace RMFileNameFixer.Service
{
    public class MainService : IMainService
    {
        private readonly ILogger _logger;
        private readonly IRenameService _renameService;
        private readonly RmFixerOptions _options;
        private readonly ISystemWrapper _systemWrapper;
        private readonly IBackupService _backupService;

        public MainService(ILogger logger, IRenameService renameService, RmFixerOptions options, ISystemWrapper systemWrapper, IBackupService backupService)
        {
            _logger = logger;
            _renameService = renameService;
            _options = options;
            _systemWrapper = systemWrapper;
            _backupService = backupService;
        }

        public void InteractUser()
        {
            if (_backupService.IsBackupAlreadyExist())
            {
                if(BackupAlreadyExistUserInteraction() == false)
                {
                    return;
                }
            }

            var uiCulture = GetCultureWithUserInteraction();
            if (uiCulture != null)
            {
                _renameService.RenameBadFiles(uiCulture);
            }
        }

        public void RestoreBackup()
        {
            _backupService.RestoreFiles();
        }

        private string? GetCultureWithUserInteraction()
        {
            var uiCulture = _systemWrapper.GetCulture();

            _logger.Information($"UI culture: {uiCulture}");
            if (!_options.Locale.ContainsKey(uiCulture))
            {
                _logger.Warning($"There isn't any locale configuration for your current system locale ({uiCulture})");
                _systemWrapper.WriteLine($"There isn't any locale configuration for your current system locale ({uiCulture})");
                _systemWrapper.WriteLine("Do you want to use the fallback locale settings? It is not guaranteed that the repair will work this way!");
                _systemWrapper.WriteLine($"Press ENTER to use {_options.FallbackLocale} locale as a fallback. Press ANY other to quit");
                var key = _systemWrapper.ReadKey();
                uiCulture = key.Key == ConsoleKey.Enter ? _options.FallbackLocale : null;
            }

            return uiCulture;
        }

        private bool BackupAlreadyExistUserInteraction()
        {
            _systemWrapper.WriteLine("A previous backup already exists!");
            _systemWrapper.WriteLine("You will lose this backup if you continue!");
            _systemWrapper.WriteLine("Press ENTER to continue, press ANY other to quit");
            
            return _systemWrapper.ReadKey().Key == ConsoleKey.Enter;
        }
    }
}
