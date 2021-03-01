using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Model.Factory;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class CloudResourceCreateService : CloudResourceServiceBase, ICloudResourceCreateService
    {
        readonly IRequestIdService _requestIdService;

        public CloudResourceCreateService(SepesDbContext db, IConfiguration config, IMapper mapper, ILogger<CloudResourceCreateService> logger, IUserService userService, ISandboxModelService sandboxModelService, IRequestIdService requestIdService)
         : base(db, config, mapper, logger, userService, sandboxModelService)
        {
            _requestIdService = requestIdService;

        }      

        public async Task<CloudResource> CreateStudySpecificResourceGroupEntryAsync(int studyId, string resourceGroupName, string region, Dictionary<string, string> tags)
        {
            var currentUser = await _userService.GetCurrentUserAsync();
            var sessionId = _requestIdService.GetRequestId();

            var resourceGroupEntry = CloudResourceFactory.CreateStudyResourceGroupEntry(currentUser, sessionId, studyId, region, resourceGroupName, tags);

            await SaveToDb(resourceGroupEntry);

            return resourceGroupEntry;
        }

        public async Task<CloudResource> CreateStudySpecificDatasetEntryAsync(int datasetId,
            int resourceGroupEntryId,
            string region,
            string resourceGroupName,
            string resourceName,
            Dictionary<string, string> tags)
        {
            var currentUser = await _userService.GetCurrentUserAsync();
            var sessionId = _requestIdService.GetRequestId();

            var resourceGroupEntry = await GetInternalAsync(resourceGroupEntryId);

            if(resourceGroupEntry == null)
            {
                throw new Exception("Could not find Resource Group entry");
            }

            var resourceGroupCreateOperation = CloudResourceOperationUtil.GetCreateOperation(resourceGroupEntry);

            if (resourceGroupCreateOperation == null)
            {
                throw new Exception("Could not find Resource Group create operation entry");
            }

            var resourceEntry = 
                CloudResourceFactory.CreateStudySpecificDatasetStorageAccountEntry(
                    currentUser, sessionId, datasetId, region,
                    resourceGroupEntryId, resourceGroupName, resourceName, tags, resourceGroupCreateOperation.Id);

            await SaveToDb(resourceEntry);

            return resourceEntry;
        }


        public async Task<CloudResource> CreateSandboxResourceGroupEntryAsync(SandboxResourceCreationAndSchedulingDto dto, string resourceGroupName)
        {
            await ValidateThatNameDoesNotExistThrowIfInvalid(resourceGroupName);

            var currentUser = await _userService.GetCurrentUserAsync();
            var sessionId = _requestIdService.GetRequestId();

            var resourceGroupEntry = CloudResourceFactory.CreateSandboxResourceGroupEntry(currentUser, sessionId, dto.SandboxId, dto.Region, resourceGroupName, dto.Tags, dto.BatchId);

            await SaveToDb(resourceGroupEntry);

            return resourceGroupEntry;
        }

        public async Task<CloudResource> CreateSandboxResourceEntryAsync(
            SandboxResourceCreationAndSchedulingDto dto,
            string resourceType,
            string resourceName,           
            string configString = null,            
            int dependsOn = 0)
        {
            await ValidateThatNameDoesNotExistThrowIfInvalid(resourceName);

            var currentUser = await _userService.GetCurrentUserAsync();
            var sessionId = _requestIdService.GetRequestId();          

            var resourceEntry = CloudResourceFactory.CreateSandboxResourceEntry(currentUser, sessionId, dto.SandboxId, dto.Region, resourceType, dto.ResourceGroup.Id, resourceName, dto.Tags, configString, dto.BatchId, dependsOn, dto.ResourceGroupName);

            await SaveToDb(resourceEntry);

            return resourceEntry;
        }

        public async Task<CloudResource> CreateVmEntryAsync(int sandboxId, CloudResource resourceGroup, string region, Dictionary<string, string> tags, string vmName, int operationDependsOn, string configString)
        {
            try
            {
                await ValidateThatNameDoesNotExistThrowIfInvalid(vmName);

                var currentUser = await _userService.GetCurrentUserAsync();
                var sessionId = _requestIdService.GetRequestId();

                var resourceEntry = CloudResourceFactory.CreateSandboxResourceEntry(
                    currentUser, sessionId,
                    sandboxId, region, AzureResourceType.VirtualMachine,
                    resourceGroup.Id, resourceName: vmName, tags: tags, configString: configString,
                    dependsOn: operationDependsOn,
                    resourceGroupName: resourceGroup.ResourceGroupName
                   );

                await SaveToDb(resourceEntry);

                return resourceEntry;
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to create database resource entry for Virtual Machine for Sandbox {sandboxId}. See inner Exception for details", ex);
            }
        }

        public async Task ValidateThatNameDoesNotExistThrowIfInvalid(string resourceName)
        {
            if (await _db.CloudResources.Where(r => r.ResourceName == resourceName && !r.Deleted).AnyAsync())
            {
                throw new Exception($"Resource with name {resourceName} allready exists!");
            }
        }

        async Task SaveToDb(CloudResource resource)
        {
            _db.CloudResources.Add(resource);
            await _db.SaveChangesAsync();           
        }

        async Task<CloudResourceDto> SaveToDbAndMap(CloudResource resource)
        {
            await SaveToDb(resource);  
            return MapEntityToDto(resource);
        }      
    }
}
