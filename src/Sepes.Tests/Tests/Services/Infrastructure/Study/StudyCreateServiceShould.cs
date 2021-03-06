﻿using Sepes.Common.Dto.Study;
using Sepes.Tests.Setup;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Sepes.Tests.Services.DomainServices
{
    public class StudyCreateServiceShould : ServiceTestBaseWithInMemoryDb
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async Task FailIfStudyIsMissingName(string name)
        {
            await ClearTestDatabase();

            var studyCreateService = StudyServiceMockFactory.CreateService(_serviceProvider);

            var studyWithInvalidName = new StudyCreateDto()
            {
                Name = name,
                Vendor = "Bouvet"
            };

            await Assert.ThrowsAsync<ArgumentException>(() => studyCreateService.CreateAsync(studyWithInvalidName));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public async void FailIfStudyIsMissingVendor(string vendor)
        {
            await ClearTestDatabase();

            var studyCreateService = StudyServiceMockFactory.CreateService(_serviceProvider);

            var studyWithInvalidVendor = new StudyCreateDto()
            {
                Name = "TestStudy",
                Vendor = vendor
            };

            await Assert.ThrowsAsync<ArgumentException>(() => studyCreateService.CreateAsync(studyWithInvalidVendor));
        }
    }
}
