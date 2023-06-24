using RMFileNameFixer.Model;
using RMFileNameFixer.Service.Interface;
using Serilog;

namespace RMFileNameFixer.Service
{
    public class RenameService : IRenameService
    {
        private readonly RmFixerOptions _options;
        private readonly IFilenameService _filenameService;
        private readonly ILogger _logger;
        private readonly ISystemWrapper _systemWrapper;
        private readonly IBackupService _backupService;

        public RenameService(RmFixerOptions options, IFilenameService filenameService, ILogger logger, ISystemWrapper systemWrapper, IBackupService backupService)
        {
            _options = options;
            _filenameService = filenameService;
            _logger = logger;
            _systemWrapper = systemWrapper;
            _backupService = backupService;
        }

        public void RenameBadFiles(string culture)
        {
            var renameList = new List<Rename>();

            foreach (var folder in _options.RmFolders)
            {
                if (_systemWrapper.DirectoryExists(folder))
                {
                    var filesToRename = _filenameService.SelectFilesToRename(folder, culture);
                    var newFilenames = _filenameService.NewFilenames(filesToRename, culture);
                    foreach (var file in newFilenames)
                    {
                        _systemWrapper.WriteLine($"{file.OriginalName} => {file.NewName}");
                        try
                        {
                            _systemWrapper.MoveFile(file.OriginalName, file.NewName);
                            renameList.Add(file);
                        }
                        catch
                        {
                            _logger.Error($"Error when trying to rename file: {file.OriginalName}");
                        }
                    }
                }
                else
                {
                    _logger.Warning($"Folder not found: {folder}");
                }
            }

            _backupService.CreateChangeLog(renameList.ToArray());
        }
    }
}