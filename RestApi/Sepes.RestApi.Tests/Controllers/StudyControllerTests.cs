using System.Collections.Generic;
using System.Net;
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
            var controller = new StudyController(databaseMock.Object, new Mock<IStudyService>().Object);
            databaseMock.Setup(db => db.createStudy("TestStudy", new int[] { 2, 3 }, new int[] { 2, 42 })).ReturnsAsync(42);

            Study study = new Study("test", 12, new List<Pod>(), new List<User>(), new List<User>(), new List<DataSet>(), false, new int[]{}, new int[]{});
            
            var result = await controller.CreationVars(study);


            Assert.Equal(42, result);
        }
        [Fact]
        public async Task StudyUpdate()
        {
            var databaseMock = new Mock<ISepesDb>();
            var controller = new StudyController(databaseMock.Object, new Mock<IStudyService>().Object);
            databaseMock.Setup(db => db.updateStudy(42, false)).ReturnsAsync(1);
            Study study = new Study("test", 12, new List<Pod>(), new List<User>(), new List<User>(), new List<DataSet>(), false, new int[]{}, new int[]{});
            var result = await controller.UpdateVars(study);

            Assert.Equal(1, result);
        }
        [Fact]
        public async Task StudyList()
        {
            var databaseMock = new Mock<ISepesDb>();
            var controller = new StudyController(databaseMock.Object, new Mock<IStudyService>().Object);
            databaseMock.Setup(db => db.getStudies(false)).ReturnsAsync("TestData");
            var result = await controller.GetStudies();

            Assert.Equal("TestData", result);
        }
        [Fact]
        public async Task GetArchived()
        {
            var databaseMock = new Mock<ISepesDb>();
            var controller = new StudyController(databaseMock.Object, new Mock<IStudyService>().Object);
            databaseMock.Setup(db => db.getStudies(true)).ReturnsAsync("TestData");
            var result = await controller.GetArchivedStudies();

            Assert.Equal("TestData", result);
        }
        [Fact]
        public async Task GetDataset()
        {
            var databaseMock = new Mock<ISepesDb>();
            var controller = new StudyController(databaseMock.Object, new Mock<IStudyService>().Object);
            databaseMock.Setup(db => db.getDatasetList()).ReturnsAsync("DatasetList");
            var result = await controller.GetDataset();
            Assert.Equal("DatasetList", result);
        }
    }

}
