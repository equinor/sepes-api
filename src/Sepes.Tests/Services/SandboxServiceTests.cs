using Microsoft.Extensions.DependencyInjection;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Tests.Setup;
using System;
using System.Linq;
using System.Threading.Tasks;
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

        async Task<StudyDto> AddStudyToTestDatabase(int studyId)
        {
            var studyService = ServiceProvider.GetService<IStudyService>();
            StudyDto study = new StudyDto()
            {
                Id = studyId,
                Name = "TestStudy",
                Vendor = "Bouvet",
                WbsCode = "1234.1345afg"
            };

            return await studyService.CreateStudyAsync(study);
        }

        [Fact]
        public async void GetSandboxesByStudyIdAsync_ShouldReturnSandboxes()
        {
            RefreshTestDb();
            ISandboxService sandboxService = ServiceProvider.GetService<ISandboxService>();
            int studyId = 1;
            await AddStudyToTestDatabase(studyId);

            var sandbox = new SandboxCreateDto() { Name = "TestSandbox" };
            var sandbox2 = new SandboxCreateDto() { Name = "TestSandbox2" };
            _ = await sandboxService.CreateAsync(studyId, sandbox);
            _ = await sandboxService.CreateAsync(studyId, sandbox2);

            var sandboxes = await sandboxService.GetSandboxesForStudyAsync(studyId);

            Assert.NotEmpty(sandboxes);
            Assert.Equal(2, sandboxes.Count());

        }

        [Fact]
        public async void AddSandboxToStudyAsync_ShouldAddSandbox()
        {
            RefreshTestDb();
            ISandboxService sandboxService = ServiceProvider.GetService<ISandboxService>();
            int studyId = 1;
            await AddStudyToTestDatabase(studyId);

            var sandbox = new SandboxCreateDto() { Name = "TestSandbox" };
            _ = await sandboxService.CreateAsync(studyId, sandbox);
            var sandboxes = await sandboxService.GetSandboxesForStudyAsync(studyId);

            Assert.NotEmpty(sandboxes);
            Assert.Single<SandboxDto>(sandboxes);
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

            var sandbox = new SandboxCreateDto() { Name = name };

            await Assert.ThrowsAsync<ArgumentException>(() => sandboxService.CreateAsync(studyID, sandbox));
        }

        [Fact]
        public async void RemoveSandboxFromStudyAsync_ShouldRemoveSandbox()
        {
            RefreshTestDb();
            ISandboxService sandboxService = ServiceProvider.GetService<ISandboxService>();
            int studyId = 1;
            await AddStudyToTestDatabase(studyId);

            var sandbox = new SandboxCreateDto() { Name = "TestSandbox" };
            _ = await sandboxService.CreateAsync(studyId, sandbox);
            var sandboxFromDb = await sandboxService.GetSandboxesForStudyAsync(studyId);
            var sandboxId = (int)sandboxFromDb.First().Id;
            _ = await sandboxService.DeleteAsync(studyId, sandboxId);

            var sandboxes = await sandboxService.GetSandboxesForStudyAsync(studyId);

            Assert.Empty(sandboxes);
        }

        [Theory]
        [InlineData(1, 420)]
        [InlineData(420, 1)]
        public async void RemoveSandboxFromStudyAsync_ShouldThrow_IfSandboxOrStudyDoesNotExist(int providedStudyId, int providedSandboxId)
        {
            RefreshTestDb();
            ISandboxService sandboxService = ServiceProvider.GetService<ISandboxService>();
            int studyId = 1;
            await AddStudyToTestDatabase(studyId);

            var sandbox = new SandboxCreateDto() { Name = "TestSandbox" };
            _ = await sandboxService.CreateAsync(studyId, sandbox);

            await Assert.ThrowsAsync<NotFoundException>(() => sandboxService.DeleteAsync(providedStudyId, providedSandboxId));
        }
    }
}
