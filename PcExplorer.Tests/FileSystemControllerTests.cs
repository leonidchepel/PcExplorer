using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PcExplorer.Services;
using Moq;
using PcExplorer.Controllers.WebApi;
using System.Net.Http;
using System.Web.Http.Routing;

namespace PcExplorer.Tests
{
    [TestClass]
    public class FileSystemControllerTests
    {
        private Mock<IJobManager<int[]>> _jobManagerMock;
        private Mock<IFileSystemService> _fileSystemServiceMock;
        private FileSystemController _controller;

        [TestInitialize]
        public void Init()
        {
            _jobManagerMock = new Mock<IJobManager<int[]>>();
            _fileSystemServiceMock = new Mock<IFileSystemService>();
            _controller = new FileSystemController(_jobManagerMock.Object, _fileSystemServiceMock.Object);
        }

        [TestMethod]
        public void ListFilesAndFolders_ShouldReturnListOfDrivesIfPathEmpty()
        {
            //arrange
            _controller.Url = new UrlHelper(new HttpRequestMessage { RequestUri = new Uri("http://test/?") });
            
            //act
            var result = _controller.ListFilesAndFolders().Result;

            //assert
            _fileSystemServiceMock.Verify(fs => fs.GetFileAndFolderList(It.IsAny<string>()), Times.Never);
            _fileSystemServiceMock.Verify(fs => fs.GetAllDrives(), Times.Once);
        }


        [TestMethod]
        public void ListFilesAndFolders_ShouldReturnListOfFilesAndFoldersIfPathNotEmpty()
        {

            //arrange
            _controller.Url = new UrlHelper(new HttpRequestMessage { RequestUri = new Uri("http://test/?not-empty") });

            //act
            var result = _controller.ListFilesAndFolders().Result;

            //assert
            _fileSystemServiceMock.Verify(fs => fs.GetAllDrives(), Times.Never);
            _fileSystemServiceMock.Verify(fs => fs.GetFileAndFolderList(It.IsAny<string>()), Times.Once);

        }

    }
}
