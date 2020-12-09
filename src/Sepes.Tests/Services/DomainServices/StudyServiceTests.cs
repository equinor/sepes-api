using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Study;
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
        ServiceCollection _services;
        ServiceProvider _serviceProvider;

        public StudyServiceTests()
        {
            _services = BasicServiceCollectionFactory.GetServiceCollectionWithInMemory();
            _services.AddTransient<IStudyService, StudyService>();
            _serviceProvider = _services.BuildServiceProvider();
        }

        void RefreshTestDb()
        {
            var db = _serviceProvider.GetService<SepesDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }


        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async void CreatingStudyWithoutNameShouldFail(string name)
        {
            RefreshTestDb();
            var studyService = StudyServiceMockFactory.Create(_serviceProvider);

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
            var studyService = StudyServiceMockFactory.Create(_serviceProvider);

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
            var studyService = StudyServiceMockFactory.Create(_serviceProvider);

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
          
            await Assert.ThrowsAsync<ArgumentException>(() => studyService.UpdateStudyMetadataAsync(studyId, studyWithoutReqFields));          
        }

        [Theory]
        [InlineData(2)]
        [InlineData(7)]
        [InlineData(99999)]
        [InlineData(123456)]
        public async void GetStudyByIdAsync_WillThrow_IfStudyDoesNotExist(int id)
        {
            RefreshTestDb();
            var studyService = StudyServiceMockFactory.Create(_serviceProvider);

            await Assert.ThrowsAsync<Infrastructure.Exceptions.NotFoundException>(() => studyService.GetStudyDtoByIdAsync(id, UserOperation.Study_Read));
        }

        [Theory]
        [InlineData(2)]
        [InlineData(7)]
        [InlineData(99999)]
        [InlineData(123456)]
        public async void DeleteStudyAsync_ShouldThrow_IfStudyDoesNotExist(int id)
        {
            RefreshTestDb();
            var studyService = StudyServiceMockFactory.Create(_serviceProvider);

            await Assert.ThrowsAsync<Infrastructure.Exceptions.NotFoundException>(() => studyService.DeleteStudyAsync(id));
        }

        [Fact]
        public async void CloseStudyAsync_ShouldClose_IfStudyExists()
        {
            RefreshTestDb();
            var studyService = StudyServiceMockFactory.Create(_serviceProvider);

            var initialStudy = new StudyCreateDto()
            {
                Name = "TestStudy12345",
                Vendor = "Equinor"
            };

            var createdStudy = await studyService.CreateStudyAsync(initialStudy);
            int studyId = createdStudy.Id;
            await studyService.CloseStudyAsync(studyId);

            _ = await Assert.ThrowsAsync<Infrastructure.Exceptions.NotFoundException>(() => studyService.GetStudyDtoByIdAsync(studyId, UserOperation.Study_Read));

        }  
    }
}
