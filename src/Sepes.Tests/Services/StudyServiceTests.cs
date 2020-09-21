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

            var updatedStudy = await studyService.UpdateStudyDetailsAsync(studyId,studyWithoutReqFields);

            Assert.NotEqual<StudyDto>(createdStudy, updatedStudy);
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

            await Assert.ThrowsAsync<Infrastructure.Exceptions.NotFoundException>(() => studyService.GetStudyByIdAsync(id));
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

            _ = await Assert.ThrowsAsync<Infrastructure.Exceptions.NotFoundException>(() => studyService.GetStudyByIdAsync(studyId));

        }

        [Theory]
        [InlineData(1, 2, "Sponsor Rep")]
        [InlineData(1, 2, "Vendor Admin")]
        [InlineData(1, 2, "Vendor Contributor")]
        [InlineData(1, 2, "Study Viewer")]
        public async void TestAddNewUser(int studyId, int expected, string role)
        {
            RefreshTestDb();
            IStudyService studyService = ServiceProvider.GetService<IStudyService>();

            var initialStudy = new StudyCreateDto()
            {
                Name = "TestStudy12345",
                Vendor = "Equinor"
            };

            var createdStudy = await studyService.CreateStudyAsync(initialStudy);

            var initialUser = new UserCreateDto()
            {
                FullName = "John",
                Role = role,
                EmailAddress = "john@test.com"
            };

            var createdParticipant = await studyService.AddNewParticipantToStudyAsync(studyId, initialUser);
            //int studyId = (int)createdStudy.Id;

            var study = studyService.GetStudyByIdAsync(studyId);


           // var updatedStudy = await studyService.UpdateStudyDetailsAsync(studyId, studyWithoutReqFields);
           // |
            Assert.Equal(expected, createdParticipant.Participants.Count);
        }

        [Theory]
        [InlineData(1, "Sponsor Re1p")]
        [InlineData(1, "Vendor Ad1min")]
        [InlineData(1, "Vendor Contr1ibutor")]
        [InlineData(1, "Study Vie1wer")]
        public async void TestAddNewUserInvalidRole(int studyId, string role)
        {
            RefreshTestDb();
            IStudyService studyService = ServiceProvider.GetService<IStudyService>();

            var initialStudy = new StudyCreateDto()
            {
                Name = "TestStudy12345",
                Vendor = "Equinor"
            };

            var createdStudy = await studyService.CreateStudyAsync(initialStudy);

            var initialUser = new UserCreateDto()
            {
                FullName = "John",
                Role = role,
                EmailAddress = "john@test.com"
            };

            await Assert.ThrowsAsync<ArgumentException>(() => studyService.AddNewParticipantToStudyAsync(studyId, initialUser));
            //Assert.Equal(expected, createdParticipant.Participants.Count);
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
