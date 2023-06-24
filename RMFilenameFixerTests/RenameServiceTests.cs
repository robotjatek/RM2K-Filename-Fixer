using Moq;

using RMFileNameFixer.Model;
using RMFileNameFixer.Service;
using RMFileNameFixer.Service.Interface;

using Serilog;

namespace RMFilenameFixerTests
{
    public class RenameServiceTests
    {
        [Fact]
        public void TestRenameBadFiles()
        {
            var options = new RmFixerOptions
            {
                RmFolders = new[] { "folder" },
                FallbackLocale = string.Empty,
                Locale = new Dictionary<string, Dictionary<string, char>>()
            };
            var filenameService = new Mock<IFilenameService>();
            var filenames = new[]
            {
                new Rename
                {
                    NewName = "foo",
                    OriginalName = "bar",
                }
            };
            filenameService.Setup(f => f.NewFilenames(It.IsAny<IEnumerable<string>>(), It.IsAny<string>())).Returns(filenames);
            var logger = new Mock<ILogger>();
            var wrapper = new Mock<ISystemWrapper>();
            wrapper.Setup(w => w.DirectoryExists(It.IsAny<string>())).Returns(true);
            var backupService = new Mock<IBackupService>();

            var sut = new RenameService(options, filenameService.Object, logger.Object, wrapper.Object, backupService.Object);
            sut.RenameBadFiles("culture");

            wrapper.Verify(w => w.MoveFile(filenames[0].OriginalName, filenames[0].NewName), Times.Once());
            backupService.Verify(b => b.CreateChangeLog(filenames), Times.Once());
        }

        [Fact]
        public void LogsWhenTheDirectoryDoesNotExists()
        {
            var options = new RmFixerOptions
            {
                RmFolders = new[] { "folder" },
                FallbackLocale = string.Empty,
                Locale = new Dictionary<string, Dictionary<string, char>>()
            };
            var filenameService = new Mock<IFilenameService>();

            var logger = new Mock<ILogger>();
            var wrapper = new Mock<ISystemWrapper>();
            wrapper.Setup(w => w.DirectoryExists(It.IsAny<string>())).Returns(false);
            var backupService = new Mock<IBackupService>();

            var sut = new RenameService(options, filenameService.Object, logger.Object, wrapper.Object, backupService.Object);
            sut.RenameBadFiles("culture");

            logger.Verify(l => l.Warning(It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void LogsWhenThereIsAnErrorDuringMovingFile()
        {
            var options = new RmFixerOptions
            {
                RmFolders = new[] { "folder" },
                FallbackLocale = string.Empty,
                Locale = new Dictionary<string, Dictionary<string, char>>()
            };
            var filenameService = new Mock<IFilenameService>();
            var filenames = new[]
            {
                new Rename
                {
                    NewName = "foo",
                    OriginalName = "bar",
                }
            };
            filenameService.Setup(f => f.NewFilenames(It.IsAny<IEnumerable<string>>(), It.IsAny<string>())).Returns(filenames);

            var logger = new Mock<ILogger>();
            var wrapper = new Mock<ISystemWrapper>();
            wrapper.Setup(w => w.DirectoryExists(It.IsAny<string>())).Returns(true);
            wrapper.Setup(w => w.MoveFile(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception());
            var backupService = new Mock<IBackupService>();

            var sut = new RenameService(options, filenameService.Object, logger.Object, wrapper.Object, backupService.Object);
            sut.RenameBadFiles("culture");

            logger.Verify(l => l.Error(It.IsAny<string>()), Times.Once());
        }
    }
}
