using Moq;

using RMFileNameFixer.Model;
using RMFileNameFixer.Service;
using RMFileNameFixer.Service.Interface;
using Serilog;

namespace RMFilenameFixerTests
{
    public class MainServiceTests
    {
        [Fact]
        public void MainServiceShouldCallRenameServiceWithFallbackCultureWhenUserAccepts()
        {
            var culture = "a-culture";

            var logger = new Mock<ILogger>();
            var renameService = new Mock<IRenameService>();
            var backupService = new Mock<IBackupService>();
            var wrapper = new Mock<ISystemWrapper>();
            var key = new ConsoleKeyInfo((char)ConsoleKey.Enter, ConsoleKey.Enter, false, false, false);
            wrapper.Setup(w => w.ReadKey()).Returns(key);
            wrapper.Setup(w => w.GetCulture()).Returns(culture);

            var options = new RmFixerOptions
            {
                Locale = new Dictionary<string, Dictionary<string, char>>(),
                RmFolders = Array.Empty<string>(),
                FallbackLocale = culture
            };

            var sut = new MainService(logger.Object, renameService.Object, options, wrapper.Object, backupService.Object);
            sut.InteractUser();

            renameService.Verify(s => s.RenameBadFiles(culture), Times.Once());
        }

        [Fact]
        public void MainServiceShouldCallQuitWhenUserDoesNotAcceptFallbackCulture()
        {
            var culture = "a-culture";

            var logger = new Mock<ILogger>();
            var renameService = new Mock<IRenameService>();
            var backupService = new Mock<IBackupService>();
            var wrapper = new Mock<ISystemWrapper>();
            var key = new ConsoleKeyInfo((char)ConsoleKey.Escape, ConsoleKey.Escape, false, false, false);
            wrapper.Setup(w => w.ReadKey()).Returns(key);
            wrapper.Setup(w => w.GetCulture()).Returns(culture);

            var options = new RmFixerOptions
            {
                Locale = new Dictionary<string, Dictionary<string, char>>(),
                RmFolders = Array.Empty<string>(),
                FallbackLocale = culture
            };

            var sut = new MainService(logger.Object, renameService.Object, options, wrapper.Object, backupService.Object);
            sut.InteractUser();

            renameService.Verify(s => s.RenameBadFiles(It.IsAny<string>()), Times.Never());
        }

        [Fact]
        public void MainServiceDoesntNeedUserInteractionWhenTheConfigurationHasTheSystemLocale()
        {
            var culture = "a-culture";

            var logger = new Mock<ILogger>();
            var renameService = new Mock<IRenameService>();
            var backupService = new Mock<IBackupService>();
            var wrapper = new Mock<ISystemWrapper>();
            wrapper.Setup(w => w.GetCulture()).Returns(culture);

            var options = new RmFixerOptions
            {
                Locale = new Dictionary<string, Dictionary<string, char>>()
                {
                    {
                        "a-culture",
                        new Dictionary<string, char>()
                    }
                },
                RmFolders = Array.Empty<string>(),
                FallbackLocale = culture
            };

            var sut = new MainService(logger.Object, renameService.Object, options, wrapper.Object, backupService.Object);
            sut.InteractUser();

            renameService.Verify(s => s.RenameBadFiles(culture), Times.Once());
            wrapper.Verify(w => w.ReadKey(), Times.Never());
        }
    }
}