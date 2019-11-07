using System.Text.Json;
using System.Threading.Tasks;
using Moq;
using Sepes.RestApi.Controller;
using Sepes.RestApi.Model;
using Sepes.RestApi.Services;
using Sepes.RestApi.Tests.Mocks;
using Xunit;

namespace Sepes.RestApi.Tests.Controller
{
    public class StudyControllerTests
    {
        [Fact]
        public async Task StudyCreate()
        {
            var databaseMock = new Mock<ISepesDb>();
            var controller = new StudyController(databaseMock.Object);
            databaseMock.Setup(db => db.createStudy("TestStudy", new int[] { 2, 3 }, new int[] { 2, 42 })).ReturnsAsync(42);
            var result = await controller.CreationVars(new Study()
            {
                studyName = "TestStudy",
                userIds = new int[] { 2, 3 },
                datasetIds = new int[] { 2, 42 }
            });


            Assert.Equal(42, result);
        }
        [Fact]
        public async Task StudyUpdate()
        {
            var databaseMock = new Mock<ISepesDb>();
            var controller = new StudyController(databaseMock.Object);
            databaseMock.Setup(db => db.updateStudy(42, false)).ReturnsAsync(1);
            var result = await controller.UpdateVars(new Study()
            {
                studyId = 42,
                archived = false
            });

            Assert.Equal(1, result);
        }
        [Fact]
        public async Task StudyList()
        {
            var databaseMock = new Mock<ISepesDb>();
            var controller = new StudyController(databaseMock.Object);
            databaseMock.Setup(db => db.getStudies(false)).ReturnsAsync("TestData");
            var result = await controller.GetStudies();

            Assert.Equal("TestData", result);
        }
        [Fact]
        public async Task GetArchived()
        {
            var databaseMock = new Mock<ISepesDb>();
            var controller = new StudyController(databaseMock.Object);
            databaseMock.Setup(db => db.getStudies(true)).ReturnsAsync("TestData");
            var result = await controller.GetArchivedStudies();

            Assert.Equal("TestData", result);
        }
        [Fact]
        public async Task GetDataset()
        {
            var databaseMock = new Mock<ISepesDb>();
            var controller = new StudyController(databaseMock.Object);
            databaseMock.Setup(db => db.getDatasetList()).ReturnsAsync("DatasetList");
            var result = await controller.GetDataset();
            Assert.Equal("DatasetList", result);
        }
    }

}
