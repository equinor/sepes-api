using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Tests.Setup;
using System;
using Xunit;

namespace Sepes.Tests.Services
{
    public class SandboxServiceTests
    {
        public ServiceCollection Services { get; private set; }
        public ServiceProvider ServiceProvider { get; protected set; }

        public SandboxServiceTests()
        {
            Services = BasicServiceCollectionFactory.GetServiceCollectionWithInMemory();
            Services.AddTransient<IStudyService, StudyService>();
            Services.AddTransient<ISandboxService, SandboxService>();
            ServiceProvider = Services.BuildServiceProvider();
        }

        void RefreshTestDb()
        {
            var db = ServiceProvider.GetService<SepesDbContext>();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();
        }

        [Theory]
        [InlineData("TestSandbox", "")]
        [InlineData("TestSandbox", null)]
        public async void AddSandboxToStudyAsync_ToStudyWithoutWbs_ShouldFail(string name, string wbs)
        {
            RefreshTestDb();
            IStudyService studyService = ServiceProvider.GetService<IStudyService>();
            ISandboxService sandboxService = ServiceProvider.GetService<ISandboxService>();

            StudyDto study = new StudyDto()
            {
                Name = "TestStudy",
                Vendor = "Bouvet",
                WbsCode = wbs
            };

            StudyDto createdStudy = await studyService.CreateStudyAsync(study);
            int studyID = (int)createdStudy.Id;

            SandboxDto sandbox = new SandboxDto() { Name = name };

            await Assert.ThrowsAsync<ArgumentException>(() => sandboxService.AddSandboxToStudyAsync(studyID, sandbox));
        }
    }
}
