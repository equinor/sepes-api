using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Migrations;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Tests.Setup;
using System;
using Xunit;

namespace Sepes.Tests.Services
{
    public class StudyServiceTests
    {
        public ServiceCollection Services { get; private set; }
        public ServiceProvider ServiceProvider { get; protected set; }

        public StudyServiceTests()
        {
            Services = BasicServiceCollectionFactory.GetServiceCollectionWithInMemory();
            Services.AddTransient<IStudyService, StudyService>();
            ServiceProvider = Services.BuildServiceProvider();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(5)]
        [InlineData(12351324)]
        public async void CreatingStudyWithSpecifiedIdShouldBeOk(int id)
        {
            var studyService = ServiceProvider.GetService<IStudyService>();
            var studyWithSpecifiedId = new StudyDto()
            {
                Id = id,
                Name = "Test",
                Vendor = "Bouvet"
            };

            var result = await studyService.CreateStudyAsync(studyWithSpecifiedId);

            Assert.IsType<int>(result.Id);
            Assert.NotEqual<int>(id, (int)result.Id);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async void CreatingStudyWithoutNameShouldFail(string name)
        {
            IStudyService studyService = ServiceProvider.GetService<IStudyService>();

            var studyWithInvalidName = new StudyDto()
            {
                Name = name,
                Vendor = "Bouvet"
            };

            await Assert.ThrowsAsync<ArgumentException>(() => studyService.CreateStudyAsync(studyWithInvalidName));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async void CreatingStudyWithoutVendorShouldFail(string vendor)
        {
            IStudyService studyService = ServiceProvider.GetService<IStudyService>();

            var studyWithInvalidVendor = new StudyDto()
            {
                Name = "TestStudy",
                Vendor = vendor
            };

            await Assert.ThrowsAsync<ArgumentException>(() => studyService.CreateStudyAsync(studyWithInvalidVendor));
        }

        [Theory]
        [InlineData(2)]
        [InlineData(7)]
        [InlineData(99999)]
        [InlineData(123456)]
        public async void GetStudyByIdWillThrowIfStudyDoesNotExist(int id)
        {
            IStudyService studyService = ServiceProvider.GetService<IStudyService>();

            await Assert.ThrowsAsync<Sepes.Infrastructure.Exceptions.NotFoundException>(() => studyService.GetStudyByIdAsync(id));
        }

        [Theory]
        [InlineData(null, "Norway", "Restricted")]
        [InlineData("TestDataset", null, "Internal")]
        [InlineData("TestDataset2", "Western Europe", null)]
        public async void CreatingStudySpecificDatasetWithoutRequiredAttributesShouldFail(string name, string location, string classification)
        {
            IStudyService studyService = ServiceProvider.GetService<IStudyService>();

            var study = new StudyDto()
            {
                Name = "TestStudy",
                Vendor = "Bouvet"
            };

            var createdStudy = await studyService.CreateStudyAsync(study);

            var datasetWithoutRequiredFields = new StudySpecificDatasetDto()
            {
                Name = name,
                Location = location,
                Classification = classification
            };

            await Assert.ThrowsAsync<ArgumentException>(() => studyService.AddStudySpecificDatasetAsync((int)createdStudy.Id, datasetWithoutRequiredFields));
        }

        [Theory]
        [InlineData("TestSandbox", "")]
        [InlineData("TestSandbox", null)]
        public async void CreatingSandboxWithoutWbsShouldFail(string name, string wbs)
        {
            IStudyService studyService = ServiceProvider.GetService<IStudyService>();

            StudyDto study = new StudyDto()
            {
                Name = "TestStudy",
                Vendor = "Bouvet",
                WbsCode = wbs
            };

            StudyDto createdStudy = await studyService.CreateStudyAsync(study);
            int studyID = (int)createdStudy.Id;

            SandboxDto sandbox = new SandboxDto() { Name = name };

            await Assert.ThrowsAsync<ArgumentException>(() => studyService.AddSandboxAsync(studyID, sandbox));
        }


        //        [Fact]
        //        public async void TestSave()
        //        {
        //            var based = new Study("test", 1);
        //            var study = new Study("test edit", 1);

        //            var dbMock = new Mock<ISepesDb>();
        //            dbMock.Setup(db => db.NewStudy(study)).ReturnsAsync(study);
        //            dbMock.Setup(db => db.NewStudy(based)).ReturnsAsync(based);
        //            dbMock.Setup(db => db.UpdateStudy(study)).ReturnsAsync(true);
        //            var podServiceMock = new Mock<IPodService>();
        //            var studyService = new StudyService(dbMock.Object, podServiceMock.Object);
        //            await studyService.Save(based, null);


        //            var savedStudy = await studyService.Save(study, based);
        //            var expected = new Study("test edit", 1);

        //            var length = studyService.GetStudies(new User("","",""), false).Count();

        //            Assert.Equal(expected, savedStudy);
        //            Assert.Equal(1, length);
        //        }


        //        [Fact]
        //        public async void TestSaveNewPod()
        //        {
        //            var based = new Study("test", 1);
        //            var newPod = new Pod(null, "test", 1);
        //            var pods = new List<Pod>();
        //            pods.Add(newPod);
        //            var study = new Study("test edit", 1, pods);

        //            var dbMock = new Mock<ISepesDb>();
        //            dbMock.Setup(db => db.NewStudy(study)).ReturnsAsync(study);
        //            dbMock.Setup(db => db.NewStudy(based)).ReturnsAsync(based);
        //            dbMock.Setup(db => db.UpdateStudy(study)).ReturnsAsync(true);
        //            var podServiceMock = new Mock<IPodService>();
        //            var studyService = new StudyService(dbMock.Object, podServiceMock.Object);

        //            await studyService.Save(based, null);
        //            var savedStudy = await studyService.Save(study, based);

        //            var expectedPodResult = new Pod(0, "test", 1);
        //            var expectedPods = new List<Pod>();
        //            expectedPods.Add(expectedPodResult);
        //            var expected = new Study("test edit", 1, expectedPods);

        //            Assert.Equal(expected.pods, savedStudy.pods);
        //            Assert.True(expected.pods.SequenceEqual(savedStudy.pods));
        //            Assert.Equal(expected, savedStudy);
        //        }

        //        [Fact]
        //        public async void TestGetStudies()
        //        {
        //            var based = new Study("test", 1);
        //            var study = new Study("test edit", 1, null, null, null, null, true);
        //            var study2 = new Study("test2", 2);
        //            var study3 = new Study("test2", 3);

        //            var dbMock = new Mock<ISepesDb>();
        //            dbMock.Setup(db => db.NewStudy(study)).ReturnsAsync(study);
        //            dbMock.Setup(db => db.NewStudy(study2)).ReturnsAsync(study2);
        //            dbMock.Setup(db => db.NewStudy(study3)).ReturnsAsync(study3);
        //            dbMock.Setup(db => db.NewStudy(based)).ReturnsAsync(based);
        //            dbMock.Setup(db => db.UpdateStudy(study)).ReturnsAsync(true);
        //            var podServiceMock = new Mock<IPodService>();
        //            var studyService = new StudyService(dbMock.Object, podServiceMock.Object);

        //            await studyService.Save(based, null);
        //            var savedStudy = await studyService.Save(study, based);
        //            var savedStudy2 = await studyService.Save(study2, null);
        //            var savedStudy3 = await studyService.Save(study3, null);

        //            int result1 = studyService.GetStudies(new User("","",""), true).Count();
        //            int result2 = studyService.GetStudies(new User("","",""), false).Count();

        //            Assert.Equal(1, result1);
        //            Assert.Equal(2, result2);
        //        }

        //        [Fact]
        //        public void TestLoadStudies()
        //        {
        //            var study1 = new Study("test", 1);
        //            var study2 = new Study("test2", 2);
        //            var study3 = new Study("test2", 3);
        //            var studies = new List<Study>();
        //            studies.Add(study1);
        //            studies.Add(study2);
        //            studies.Add(study3);

        //            var dbMock = new Mock<ISepesDb>();
        //            dbMock.Setup(db => db.GetAllStudies()).ReturnsAsync(studies);
        //            var podServiceMock = new Mock<IPodService>();
        //            var studyService = new StudyService(dbMock.Object, podServiceMock.Object);
        //            studyService.LoadStudies();

        //            int result = studyService.GetStudies(new User("","",""), false).Count();

        //            Assert.Equal(3, result);
        //        }

    }
}
