using RMFileNameFixer.Model;

namespace RMFileNameFixer.Service.Interface
{
    public interface IBackupService
    {
        void CreateChangeLog(Rename[] renamedFiles);
        void RestoreFiles();
        bool IsBackupAlreadyExist();
    }
}
