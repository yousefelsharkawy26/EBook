using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using Digital_Library.Service.Interface;
using Assert = Xunit.Assert;

namespace Digital_Library.Tests
{
    public class FileServiceTests
    {
        private readonly Mock<IFileService> _mockFileService;

        public FileServiceTests()
        {
            _mockFileService = new Mock<IFileService>();
        }

        [Fact]
        public async Task AddFile_ShouldReturnFilePath()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            var folderName = "uploads";
            var expectedPath = "uploads/test.txt";

            _mockFileService
                .Setup(s => s.AddFile(mockFile.Object, folderName))
                .ReturnsAsync(expectedPath);

            // Act
            var result = await _mockFileService.Object.AddFile(mockFile.Object, folderName);

            // Assert
            Assert.Equal(expectedPath, result);
        }

        [Fact]
        public async Task DeleteFile_ShouldReturnTrue()
        {
            // Arrange
            var fileName = "test.txt";

            _mockFileService
                .Setup(s => s.DeleteFile(fileName))
                .ReturnsAsync(true);

            // Act
            var result = await _mockFileService.Object.DeleteFile(fileName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetFile_ShouldReturnByteArray()
        {
            // Arrange
            var fileName = "test.txt";
            var expectedContent = new byte[] { 1, 2, 3 };

            _mockFileService
                .Setup(s => s.GetFile(fileName))
                .ReturnsAsync(expectedContent);

            // Act
            var result = await _mockFileService.Object.GetFile(fileName);

            // Assert
            Assert.Equal(expectedContent, result);
        }

        [Fact]
        public async Task FileExists_ShouldReturnTrue()
        {
            // Arrange
            var fileName = "test.txt";

            _mockFileService
                .Setup(s => s.FileExists(fileName))
                .ReturnsAsync(true);

            // Act
            var result = await _mockFileService.Object.FileExists(fileName);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task GetFilesInFolder_ShouldReturnListOfFiles()
        {
            // Arrange
            var folderName = "uploads";
            var expectedFiles = new List<string> { "file1.txt", "file2.txt" };

            _mockFileService
                .Setup(s => s.GetFilesInFolder(folderName))
                .ReturnsAsync(expectedFiles);

            // Act
            var result = await _mockFileService.Object.GetFilesInFolder(folderName);

            // Assert
            Assert.Equal(2, ((List<string>)result).Count);
            Assert.Contains("file1.txt", result);
        }

        [Fact]
        public async Task UpdateFile_ShouldReturnUpdatedPath()
        {
            // Arrange
            var mockFile = new Mock<IFormFile>();
            var existingFileName = "old.txt";
            var folderName = "uploads";
            var expectedPath = "uploads/old.txt";

            _mockFileService
                .Setup(s => s.UpdateFile(mockFile.Object, existingFileName, folderName))
                .ReturnsAsync(expectedPath);

            // Act
            var result = await _mockFileService.Object.UpdateFile(mockFile.Object, existingFileName, folderName);

            // Assert
            Assert.Equal(expectedPath, result);
        }
    }
}
