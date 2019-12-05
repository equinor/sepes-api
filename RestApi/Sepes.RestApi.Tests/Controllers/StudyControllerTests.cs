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
        public void StudyUpdate()
        {
            var databaseMock = new Mock<ISepesDb>();
            var controller = new StudyController(databaseMock.Object, new Mock<IStudyService>().Object);
            Study study = new Study("test", 12, new List<Pod>(), new List<User>(), new List<User>(), new List<DataSet>(), false, new int[]{}, new int[]{});

            databaseMock.Setup(db => db.UpdateStudy(study)).ReturnsAsync(true);
            var result = controller.UpdateVars(study);

            Assert.Equal(1, result);
        }

        [Fact]
        public void StudyList()
        {
            //_studyService.GetStudies(new User("","",""), false)
            var databaseMock = new Mock<ISepesDb>();
            var studyServiceMock = new Mock<IStudyService>();
            var controller = new StudyController(databaseMock.Object, studyServiceMock.Object);
            studyServiceMock.Setup(s => s.GetStudies(new User("","",""), false)).Returns(new List<Study>());
            var result = controller.GetStudies();

            Assert.Equal(new List<StudyInput>(), result);
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
