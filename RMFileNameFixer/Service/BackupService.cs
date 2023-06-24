using RMFileNameFixer.Model;
using RMFileNameFixer.Service.Interface;

namespace RMFileNameFixer.Service
{
    public class BackupService : IBackupService
    {
        private const string RENAME_LOG_PATH = "BAK/rename_log.json";
        private readonly ISystemWrapper _systemWrapper;

        public BackupService(ISystemWrapper systemWrapper)
        {
            _systemWrapper = systemWrapper;
        }

        public void CreateChangeLog(Rename[] renamedFiles)
        {
            if (renamedFiles.Any())
            {
                var json = _systemWrapper.Serialize(renamedFiles);
                _systemWrapper.CreateFile(RENAME_LOG_PATH, json);
            }
        }

        public bool IsBackupAlreadyExist()
        {
            return _systemWrapper.FileExist(RENAME_LOG_PATH);
        }

        public void RestoreFiles()
        {
            if (_systemWrapper.FileExist(RENAME_LOG_PATH))
            {
                var renameLog = _systemWrapper.Deserialize<Rename[]>(RENAME_LOG_PATH)?.ToList();
                renameLog?.ForEach(r =>
                {
                    _systemWrapper.MoveFile(r.NewName, r.OriginalName);
                });
                _systemWrapper.DeleteFile(RENAME_LOG_PATH);
            }
        }
    }
}
