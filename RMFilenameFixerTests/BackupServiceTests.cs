using FluentAssertions;

using Moq;

using RMFileNameFixer.Model;
using RMFileNameFixer.Service;
using RMFileNameFixer.Service.Interface;

namespace RMFilenameFixerTests
{
    public class BackupServiceTests
    {
        private const string RENAMELOG_PATH = "BAK/rename_log.json";

        [Fact]
        public void IsBackupAlreadyExistShouldReturnTrue()
        {
            var wrapper = new Mock<ISystemWrapper>();
            wrapper.Setup(w => w.FileExist(RENAMELOG_PATH)).Returns(true);
            var sut = new BackupService(wrapper.Object);

            var result = sut.IsBackupAlreadyExist();
            result.Should().BeTrue();
        }

        [Fact]
        public void IsBackupAlreadyExistShouldReturnFalse()
        {
            var wrapper = new Mock<ISystemWrapper>();
            wrapper.Setup(w => w.FileExist(RENAMELOG_PATH)).Returns(false);
            var sut = new BackupService(wrapper.Object);

            var result = sut.IsBackupAlreadyExist();
            result.Should().BeFalse();
        }

        [Fact]
        public void TestRestoreFiles()
        {
            var wrapper = new Mock<ISystemWrapper>();
            wrapper.Setup(w => w.FileExist(RENAMELOG_PATH)).Returns(true);
            var renamedFiles = new[]
            {
                new Rename
                {
                    NewName = "new1",
                    OriginalName = "old1"
                },
                new Rename
                {
                    NewName = "new2",
                    OriginalName = "old2"
                }
            };
            wrapper.Setup(w => w.Deserialize<Rename[]>(It.IsAny<string>())).Returns(renamedFiles);
            var sut = new BackupService(wrapper.Object);

            sut.RestoreFiles();

            wrapper.Verify(w => w.MoveFile(renamedFiles[0].NewName, renamedFiles[0].OriginalName), Times.Once());
            wrapper.Verify(w => w.MoveFile(renamedFiles[1].NewName, renamedFiles[1].OriginalName), Times.Once());
            wrapper.Verify(w => w.DeleteFile(RENAMELOG_PATH));
        }

        [Fact]
        public void TestRestoreFilesWithEmptyArray()
        {
            var wrapper = new Mock<ISystemWrapper>();
            wrapper.Setup(w => w.FileExist(RENAMELOG_PATH)).Returns(true);
            wrapper.Setup(w => w.Deserialize<Rename[]>(It.IsAny<string>())).Returns(Array.Empty<Rename>());
            var sut = new BackupService(wrapper.Object);

            sut.RestoreFiles();
            wrapper.Verify(w => w.FileExist(RENAMELOG_PATH));
            wrapper.Verify(w => w.Deserialize<Rename[]>(RENAMELOG_PATH));
            wrapper.Verify(w => w.DeleteFile(RENAMELOG_PATH));
            wrapper.VerifyNoOtherCalls();
        }

        [Fact]
        public void DoesNothingWhenThereIsNoBackup()
        {
            var wrapper = new Mock<ISystemWrapper>();
            wrapper.Setup(w => w.FileExist(RENAMELOG_PATH)).Returns(false);
            var sut = new BackupService(wrapper.Object);

            sut.RestoreFiles();
            wrapper.Verify(w => w.FileExist(It.IsAny<string>()));
            wrapper.VerifyNoOtherCalls();
        }

        [Fact]
        public void TestCreateChangeLog()
        {
            var wrapper = new Mock<ISystemWrapper>();
            var sut = new BackupService(wrapper.Object);
            var changelog = new[]
            {
                new Rename
                {
                    NewName = "foo",
                    OriginalName = "bar",
                }
            };
            sut.CreateChangeLog(changelog);

            wrapper.Verify(w => w.Serialize(changelog));
            wrapper.Verify(w => w.CreateFile(RENAMELOG_PATH, It.IsAny<string>()), Times.Once());
        }

        [Fact]
        public void TestCreateChangeLogDoesNothingWhenThereAreNoChanges()
        {
            var wrapper = new Mock<ISystemWrapper>();
            var sut = new BackupService(wrapper.Object);
            sut.CreateChangeLog(Array.Empty<Rename>());

            wrapper.Verify(w => w.Serialize(It.IsAny<Rename>()), Times.Never());
            wrapper.Verify(w => w.CreateFile(RENAMELOG_PATH, It.IsAny<string>()), Times.Never());
        }
    }
}
