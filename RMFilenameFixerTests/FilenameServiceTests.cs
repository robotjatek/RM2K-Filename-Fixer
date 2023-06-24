using FluentAssertions;

using Moq;

using RMFileNameFixer.Model;
using RMFileNameFixer.Service;
using RMFileNameFixer.Service.Interface;

namespace RMFilenameFixerTests
{
    public class FilenameServiceTests
    {
        [Fact]
        public void FindsFilenamesWithReplaceableChars()
        {
            var wrapper = new Mock<ISystemWrapper>();
            wrapper.Setup(w => w.GetFiles(It.IsAny<string>())).Returns(new[]
            {
                "XAB",
                "BBBBBB",
            });

            var options = new RmFixerOptions
            {
                Locale = new Dictionary<string, Dictionary<string, char>>()
                {
                    {
                        "mockLocale",
                        new Dictionary<string, char>
                        {
                            { "A", 'C' }
                        }
                    }
                },
                RmFolders = Array.Empty<string>(),
                FallbackLocale = "en-US"
            };

            var sut = new FilenameService(options, wrapper.Object);
            var filenames = sut.SelectFilesToRename("", "mockLocale").ToArray();
            filenames.Should().HaveCount(1);
            filenames[0].Should().Be("XAB");
        }

        [Fact]
        public void FindsMultipleFilenamesWithReplaceableChars()
        {
            var wrapper = new Mock<ISystemWrapper>();
            wrapper.Setup(w => w.GetFiles(It.IsAny<string>())).Returns(new[]
            {
                "XAB",
                "BBBBBB",
                "ASD"
            });

            var options = new RmFixerOptions
            {
                Locale = new Dictionary<string, Dictionary<string, char>>()
                {
                    {
                        "mockLocale",
                        new Dictionary<string, char>
                        {
                            { "A", 'C' }
                        }
                    }
                },
                RmFolders = Array.Empty<string>(),
                FallbackLocale = "en-US"
            };

            var sut = new FilenameService(options, wrapper.Object);
            var filenames = sut.SelectFilesToRename("", "mockLocale").ToArray();
            filenames.Should().HaveCount(2);
            filenames[0].Should().Be("XAB");
            filenames[1].Should().Be("ASD");
        }

        [Fact]
        public void ReplacesCharInFoundFilename()
        {
            var filesToRename = new[]
            {
                "AAAA",
                "BABA"
            };

            var wrapper = new Mock<ISystemWrapper>();
            var options = new RmFixerOptions
            {
                Locale = new Dictionary<string, Dictionary<string, char>>()
                {
                    {
                        "mockLocale",
                        new Dictionary<string, char>
                        {
                            { "A", 'C' }
                        }
                    }
                },
                RmFolders = Array.Empty<string>(),
                FallbackLocale = "en-US"
            };

            var sut = new FilenameService(options, wrapper.Object);
            var filenames = sut.NewFilenames(filesToRename, "mockLocale").ToArray();
            filenames.Should().HaveCount(2);
            filenames[0].NewName.Should().Be("CCCC");
            filenames[1].NewName.Should().Be("BCBC");
        }
    }
}
