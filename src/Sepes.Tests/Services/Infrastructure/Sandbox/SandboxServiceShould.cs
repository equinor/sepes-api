﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Sepes.Common.Constants;
using Sepes.Common.Dto.Sandbox;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Tests.Common.Constants;
using Sepes.Tests.Common.ModelFactory;
using Sepes.Tests.Setup;
using Xunit;

namespace Sepes.Tests.Services.Infrastructure
{
    public class SandboxServiceShould : ServiceTestBase
    {
        //[Fact]
        //public async Task CreateSandbox_When_Required_Properties_Provided()
        //{
        //    var study = StudyModelFactory.CreateBasic();
        //    var sandboxCreateRequest = CreateSandboxRequest();
        //    var serviceMockPackage = CreateMockPackageForSuccess(study);

        //    var createdSandbox = await serviceMockPackage.SandboxService.CreateAsync(study.Id, sandboxCreateRequest);
            
        //    Assert.Equal(study.Id, createdSandbox.StudyId);
        //}
        
          
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async void Throw_When_Sandbox_Missing_Name(string sandboxName)
        {
            var sandboxService = CreateForFailingSandboxCreate(AppRoles.Admin, 1);
            
            var sandboxCreateRequest = CreateSandboxRequest(sandboxName);

            await Assert.ThrowsAsync<ArgumentException>(() => sandboxService.CreateAsync(StudyConstants.CREATED_BY_ME_ID, sandboxCreateRequest));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async void Throw_When_Sandbox_Missing_Region(string sandboxRegion)
        {
            var sandboxService = CreateForFailingSandboxCreate(AppRoles.Admin, 1);
            
            var sandboxCreateRequest = CreateSandboxRequest(region: sandboxRegion);

            await Assert.ThrowsAsync<ArgumentException>(() => sandboxService.CreateAsync(StudyConstants.CREATED_BY_ME_ID, sandboxCreateRequest));
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async void Throw_When_Study_Missing_Wbs(string wbs)
        {
            var sandboxService = CreateForFailingSandboxCreate(AppRoles.Admin, 1, wbs: wbs);
            
            var sandboxCreateRequest = CreateSandboxRequest();

            await Assert.ThrowsAsync<ArgumentException>(() => sandboxService.CreateAsync(StudyConstants.CREATED_BY_ME_ID, sandboxCreateRequest));
        }
        
        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public async void Throw_When_Study_Invalid_Wbs(string wbs)
        {
            var sandboxService = CreateForFailingSandboxCreate(AppRoles.Admin, 1, wbs: wbs);
            
            var sandboxCreateRequest = CreateSandboxRequest();

            await Assert.ThrowsAsync<ArgumentException>(() => sandboxService.CreateAsync(StudyConstants.CREATED_BY_ME_ID, sandboxCreateRequest));
        }
        
        SandboxCreateDto CreateSandboxRequest(string name = "newSandbox", string region = "norwayeast")
        {
            return new SandboxCreateDto() { Name = name, Region = region };
        }

        SandboxServicesAndMocks CreateMockPackageForSuccess(Study study)
        {
            var mockPackage = CreateMockPackageWithExistingStudy(study);

            mockPackage.SandboxModelServiceMock
             .Setup(x =>
                 x.AddAsync(It.IsAny<Study>(), It.IsAny<Sandbox>())
                 )
             .ReturnsAsync((Study a, Sandbox b) =>
             {
                 b.Study = a;
                 b.StudyId = a.Id;
                 a.Sandboxes = new List<Sandbox>() {b};
                 return b;
             });
          
            return mockPackage;
        }

        SandboxServicesAndMocks CreateMockPackageWithExistingStudy(Study study)
        {
            var studies = new List<Study>() { study };
            
            //STUDY MODEL SERVICE
            var studyModelServiceMock = new Mock<IStudyModelService>();
            
            studyModelServiceMock
                .Setup(x => 
                    x.GetForSandboxCreateAndDeleteAsync(It.IsAny<int>(), It.IsAny<UserOperation>()))
                .ReturnsAsync((int a, UserOperation b) => studies?.FirstOrDefault(s => s.Id == a));

            //SANDBOX MODEL SERVICE
            var sandboxModelServiceMock = new Mock<ISandboxModelService>();

            var  sandboxService = SandboxServiceWithMocksFactory.Create(_serviceProvider, AppRoles.Admin, 1, studyModelServiceMock.Object,  sandboxModelServiceMock.Object);
            
            return new SandboxServicesAndMocks(sandboxService, studyModelServiceMock, sandboxModelServiceMock);
        }
        
        ISandboxService CreateForFailingSandboxCreate(string userAppRole,
            int userId,
            int studyId = StudyConstants.CREATED_BY_ME_ID,
            string wbs = StudyConstants.CREATED_BY_ME_WBS)
        {
            var study = StudyModelFactory.CreateBasic(id: studyId, wbs: wbs);
            return SandboxServiceWithMocksFactory.ForSandboxCreate(_serviceProvider, userAppRole, userId, new List<Study>() {study});
        }
        
        class SandboxServicesAndMocks
        {
            public SandboxServicesAndMocks(ISandboxService sandboxService, Mock<IStudyModelService> studyModelServiceMock, Mock<ISandboxModelService> sandboxModelServiceMock)
            {
                SandboxService = sandboxService;
                StudyModelServiceMock = studyModelServiceMock;
                SandboxModelServiceMock = sandboxModelServiceMock;
            }

            public ISandboxService SandboxService { get; private set; }
            
            public Mock<IStudyModelService> StudyModelServiceMock{ get; private set; }
            
            public Mock<ISandboxModelService> SandboxModelServiceMock{ get; private set; }
        }
    }

   
}