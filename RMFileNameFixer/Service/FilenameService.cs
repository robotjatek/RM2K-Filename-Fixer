using RMFileNameFixer.Model;
using RMFileNameFixer.Service.Interface;
using System.Text;

namespace RMFileNameFixer.Service
{
    public class FilenameService : IFilenameService
    {
        private readonly RmFixerOptions _options;
        private readonly ISystemWrapper _systemWrapper;

        public FilenameService(RmFixerOptions options, ISystemWrapper systemWrapper)
        {
            _options = options;
            _systemWrapper = systemWrapper;
        }

        public IEnumerable<string> SelectFilesToRename(string folder, string culture)
        {
            var charsToReplace = _options.Locale[culture].Keys;
            var filenames = _systemWrapper.GetFiles(folder);
            return filenames.Where(filename =>
            {
                return charsToReplace.Any(c => filename.Contains(c));
            }).ToList();
        }

        public IEnumerable<Rename> NewFilenames(IEnumerable<string> filesToRename, string culture)
        {
            foreach (var file in filesToRename)
            {
                var toReplaceChars = _options.Locale[culture];
                var tempFilename = new StringBuilder(file, file.Length);
                foreach (var toReplaceChar in toReplaceChars)
                {
                    tempFilename.Replace(toReplaceChar.Key[0], toReplaceChar.Value);
                }

                yield return new Rename
                {
                    OriginalName = file,
                    NewName = tempFilename.ToString()
                };
            }
        }
    }
}
