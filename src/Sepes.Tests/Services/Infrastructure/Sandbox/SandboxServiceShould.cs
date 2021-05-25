using System.Threading.Tasks;
using Sepes.Common.Constants;
using Sepes.Common.Dto.Sandbox;
using Sepes.Tests.Setup;
using Xunit;

namespace Sepes.Tests.Services.Infrastructure.Sandbox
{
    public class SandboxServiceShould : ServiceTestBase
    {
        [Fact]
        public async Task CreateSandbox()
        {
            int studyId = 1;
            var sandboxService = SandboxServiceWithMocksFactory.Create(_serviceProvider, AppRoles.Admin, 1);
            
            var sandboxCreateRequest = new SandboxCreateDto() { Name = "testsandbox", Region = "norwayeast"};
            var createdSandbox = await sandboxService.CreateAsync(studyId, sandboxCreateRequest);
            
            Assert.Equal(studyId, createdSandbox.StudyId);
        }
        
        //[Theory]
        //[InlineData("TestSandbox", "")]
        //[InlineData("TestSandbox", null)]
        //public async void AddSandboxToStudyAsync_ToStudyWithoutWbs_ShouldFail(string name, string wbs)
        //{
        //    RefreshTestDb();
        //    IStudyService studyService = ServiceProvider.GetService<IStudyService>();
        //    ISandboxService sandboxService = ServiceProvider.GetService<ISandboxService>();

        //    StudyCreateDto study = new StudyCreateDto()
        //    {
        //        Name = "TestStudy",
        //        Vendor = "Bouvet",
        //        WbsCode = wbs
        //    };

        //    StudyDto createdStudy = await studyService.CreateStudyAsync(study);
        //    int studyID = (int)createdStudy.Id;

        //    var sandbox = new SandboxCreateDto() { Name = name };

        //    await Assert.ThrowsAsync<ArgumentException>(() => sandboxService.CreateAsync(studyID, sandbox));
        //}
    }
}