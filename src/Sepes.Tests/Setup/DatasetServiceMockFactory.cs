using AutoMapper;
using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Sepes.Infrastructure.Dto.Azure;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Tests.Util;
using System.Collections.Generic;
using System.Threading;

namespace Sepes.Tests.Setup
{

    public static class DatasetServiceMockFactory
    {
        public static IStudyDatasetService GetStudyDatasetService(ServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetService<SepesDbContext>();
            var config = serviceProvider.GetService<IConfiguration>();
            var mapper = serviceProvider.GetService<IMapper>();
            var logger = serviceProvider.GetService<ILogger<StudyDatasetService>>();
            var userService = UserFactory.GetUserServiceMockForAdmin(1);

            var rgServiceMock = new Mock<IAzureResourceGroupService>();
            rgServiceMock.SetReturnsDefault<AzureResourceGroupDto>(new AzureResourceGroupDto() { Id = "rgid", Name = "rgname", ResourceGroupName = "rgname" });

            var storageAccountServiceMock = new Mock<IAzureStorageAccountService>();
            storageAccountServiceMock.Setup(x => x.CreateStorageAccount(It.IsAny<Region>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Dictionary<string, string>>(), It.IsAny<List<string>>(), default(CancellationToken))).ReturnsAsync(AzureResourceDtoFactory.CreateStorageAccount());

            var roleAssignmentServiceMock = new Mock<IAzureRoleAssignmentService>();
            roleAssignmentServiceMock.Setup(x => x.AddResourceRoleAssignment(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), default(CancellationToken))).ReturnsAsync(AzureResourceDtoFactory.CreateAzureRoleAssignmentResponseDto());


            return new StudyDatasetService(config, db, mapper, logger, userService.Object, rgServiceMock.Object, storageAccountServiceMock.Object, roleAssignmentServiceMock.Object);          
        }
    }
}
