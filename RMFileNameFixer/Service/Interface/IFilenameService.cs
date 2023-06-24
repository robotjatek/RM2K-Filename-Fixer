using RMFileNameFixer.Model;

namespace RMFileNameFixer.Service.Interface
{
    public interface IFilenameService
    {
        IEnumerable<string> SelectFilesToRename(string folder, string culture);
        public IEnumerable<Rename> NewFilenames(IEnumerable<string> filesToRename, string culture);
    }
}