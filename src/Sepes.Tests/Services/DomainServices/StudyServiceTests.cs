using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model.Context;
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

        void RefreshTestDb()
        {
            var db = ServiceProvider.GetService<SepesDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async void CreatingStudyWithoutNameShouldFail(string name)
        {
            RefreshTestDb();
            IStudyService studyService = ServiceProvider.GetService<IStudyService>();

            var studyWithInvalidName = new StudyCreateDto()
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
            RefreshTestDb();
            IStudyService studyService = ServiceProvider.GetService<IStudyService>();

            var studyWithInvalidVendor = new StudyCreateDto()
            {
                Name = "TestStudy",
                Vendor = vendor
            };

            await Assert.ThrowsAsync<ArgumentException>(() => studyService.CreateStudyAsync(studyWithInvalidVendor));
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "null")]
        [InlineData(null, "Bouvet")]
        [InlineData("", "Bouvet")]
        [InlineData("TestStudy", null)]
        [InlineData("TestStudy", "")]
        public async void UpdatingStudyDetailsWithoutRequiredFieldsShouldBeWellHandled(string name, string vendor)
        {
            RefreshTestDb();
            IStudyService studyService = ServiceProvider.GetService<IStudyService>();

            var initialStudy = new StudyCreateDto()
            {
                Name = "TestStudy12345",
                Vendor = "Equinor"
            };

            var createdStudy = await studyService.CreateStudyAsync(initialStudy);
            int studyId = (int)createdStudy.Id;

            var studyWithoutReqFields = new StudyDto()
            {
                Id = studyId,
                Name = name,
                Vendor = vendor
            };
          
            await Assert.ThrowsAsync<ArgumentException>(() => studyService.UpdateStudyDetailsAsync(studyId, studyWithoutReqFields));          
        }

        [Theory]
        [InlineData(2)]
        [InlineData(7)]
        [InlineData(99999)]
        [InlineData(123456)]
        public async void GetStudyByIdAsync_WillThrow_IfStudyDoesNotExist(int id)
        {
            RefreshTestDb();
            IStudyService studyService = ServiceProvider.GetService<IStudyService>();

            await Assert.ThrowsAsync<Infrastructure.Exceptions.NotFoundException>(() => studyService.GetStudyDtoByIdAsync(id, UserOperations.StudyRead));
        }

        [Theory]
        [InlineData(2)]
        [InlineData(7)]
        [InlineData(99999)]
        [InlineData(123456)]
        public async void DeleteStudyAsync_ShouldThrow_IfStudyDoesNotExist(int id)
        {
            RefreshTestDb();
            IStudyService studyService = ServiceProvider.GetService<IStudyService>();

            await Assert.ThrowsAsync<Infrastructure.Exceptions.NotFoundException>(() => studyService.DeleteStudyAsync(id));
        }

        [Fact]
        public async void DeleteStudyAsync_ShouldDelete_IfStudyExists()
        {
            RefreshTestDb();
            IStudyService studyService = ServiceProvider.GetService<IStudyService>();

            var initialStudy = new StudyCreateDto()
            {
                Name = "TestStudy12345",
                Vendor = "Equinor"
            };

            var createdStudy = await studyService.CreateStudyAsync(initialStudy);
            int studyId = createdStudy.Id.Value;
            _ = await studyService.DeleteStudyAsync(studyId);

            _ = await Assert.ThrowsAsync<Infrastructure.Exceptions.NotFoundException>(() => studyService.GetStudyDtoByIdAsync(studyId, UserOperations.StudyRead));

        }

       


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

     

    }
}
