using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Interface;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Model.Factory;
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

        public CloudResourceCreateService(SepesDbContext db, IConfiguration config, IMapper mapper, ILogger<CloudResourceCreateService> logger, IUserService userService, IRequestIdService requestIdService)
         : base(db, config, mapper, logger, userService)
        {
            _requestIdService = requestIdService;

        }

        public async Task<CloudResourceDto> Create(SandboxResourceCreationAndSchedulingDto dto, string type, string resourceName, bool sandboxControlled = true, string configString = null, int operationDependsOn = 0)
        {
            _logger.LogInformation($"Creating Resource entry for sandbox: {dto.SandboxId}, resource type: {type}");

            var newResource = await AddSandboxResourceInternal(dto.SandboxId, type, dto.Region.Name, dto.ResourceGroupName, resourceName, dto.Tags, dto.ResourceGroup != null ? dto.ResourceGroup.Id : 0, sandboxControlled: sandboxControlled, operationDependsOn: operationDependsOn, configString: configString, batchId: dto.BatchId);

            var mappedToDto = MapEntityToDto(newResource);

            return mappedToDto;
        }

        public async Task<CloudResourceDto> CreateStudySpecificResourceGroupEntryAsync(int studyId, string resourceGroupName, string region, Dictionary<string, string> tags)
        {
            var currentUser = await _userService.GetCurrentUserAsync();
            var sessionId = _requestIdService.GetRequestId();

            var resourceGroupEntry = CloudResourceFactory.CreateStudyResourceGroupEntry(currentUser, sessionId, studyId, region, resourceGroupName, tags);

            return await SaveToDbAndMap(resourceGroupEntry);
        }     

        public async Task<CloudResourceDto> CreateSandboxResourceGroupEntryAsync(SandboxResourceCreationAndSchedulingDto dto, string resourceGroupName)
        {
            var currentUser = await _userService.GetCurrentUserAsync();
            var sessionId = _requestIdService.GetRequestId();

            var resourceGroupEntry = CloudResourceFactory.CreateSandboxResourceGroupEntry(currentUser, sessionId, dto.SandboxId, dto.Region.Name, resourceGroupName, dto.Tags, dto.BatchId);

            return await SaveToDbAndMap(resourceGroupEntry);
        }

        public async Task<CloudResourceDto> CreateVmEntryAsync(int sandboxId, CloudResource resourceGroup, string region, Dictionary<string, string> tags, string vmName, int operationDependsOn, string configString)
        {
            try
            {
                var currentUser = await _userService.GetCurrentUserAsync();
                var sessionId = _requestIdService.GetRequestId();

                var resourceGroupEntry = CloudResourceFactory.CreateSandboxResourceEntry(
                    currentUser, sessionId,
                    sandboxId, region, AzureResourceType.VirtualMachine,
                    resourceGroup.Id, resourceGroup.ResourceGroupName, vmName, tags,
                    configString: configString,
                    dependsOn: operationDependsOn
                   );

                return await SaveToDbAndMap(resourceGroupEntry);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to create database resource entry for Virtual Machine for Sandbox {sandboxId}. See inner Exception for details", ex);
            }
        }

        public async Task ValidateThatNameDoesNotExistThrowIfInvalid(string resourceName)
        {
            if (await _db.CloudResources.Where(r => r.ResourceName == resourceName && r.Deleted == false).AnyAsync())
            {
                throw new Exception($"Resource with name {resourceName} allready exists!");
            }
        }

        async Task<CloudResourceDto> SaveToDbAndMap(CloudResource resource)
        {
            _db.CloudResources.Add(resource);
            await _db.SaveChangesAsync();

            return MapEntityToDto(resource);
        }

       

        async Task<CloudResource> GetBasicResourceEntryWithCreateOperationInternal(string resourceType, string region, string resourceGroupName, string resourceName, Dictionary<string, string> tags, string configString = null,
           int? studyId = default, int? sandboxId = default, int? parentResourceId = default, bool sandboxControlled = true,
          string createOperationDescription = null, int? operationDependsOn = default,

            string batchId = null)
        {

            var currentUser = await _userService.GetCurrentUserAsync();

            var tagsString = AzureResourceTagsFactory.TagDictionaryToString(tags);

            var newResource = new CloudResource()
            {
                ResourceType = resourceType,
                Region = region,
                ResourceGroupName = resourceGroupName,
                ResourceName = resourceName,
                Tags = tagsString,
                ConfigString = configString,
                ResourceKey = AzureResourceNameUtil.AZURE_RESOURCE_INITIAL_ID_OR_NAME,
                ResourceId = AzureResourceNameUtil.AZURE_RESOURCE_INITIAL_ID_OR_NAME,
                StudyId = studyId != 0 ? studyId : default,
                SandboxId = sandboxId != 0 ? sandboxId : default,
                ParentResourceId = parentResourceId != 0 ? parentResourceId : default,
                SandboxControlled = sandboxControlled,
                Operations = new List<CloudResourceOperation> {
                    new CloudResourceOperation()
                    {
                    Description = createOperationDescription,
                    BatchId = batchId,
                    OperationType = CloudResourceOperationType.CREATE,
                    CreatedBy = currentUser.UserName,
                    CreatedBySessionId = _requestIdService.GetRequestId(),
                    DependsOnOperationId = operationDependsOn != 0 ? operationDependsOn: default(int?),
                    MaxTryCount = CloudResourceConstants.RESOURCE_MAX_TRY_COUNT
                    }
                },
                CreatedBy = currentUser.UserName
            };

            return newResource;
        }

        async Task<CloudResource> AddSandboxResourceInternal(int sandboxId, string type, string region, string resourceGroupName, string resourceName, Dictionary<string, string> tags, int? parentResourceId = default, bool sandboxControlled = true, int? operationDependsOn = default, string configString = null, string batchId = null)
        {
            try
            {
                await ValidateThatNameDoesNotExistThrowIfInvalid(resourceName);

                var sandboxFromDb = await GetSandboxOrThrowAsync(sandboxId);

                var tagsString = AzureResourceTagsFactory.TagDictionaryToString(tags);

                var currentUser = await _userService.GetCurrentUserAsync();

                var newResource = new CloudResource()
                {
                    ResourceGroupName = resourceGroupName,
                    ResourceType = type,
                    ResourceKey = AzureResourceNameUtil.AZURE_RESOURCE_INITIAL_ID_OR_NAME,
                    ResourceName = resourceName,
                    ResourceId = AzureResourceNameUtil.AZURE_RESOURCE_INITIAL_ID_OR_NAME,
                    SandboxControlled = sandboxControlled,
                    Region = region,
                    Tags = tagsString,
                    ConfigString = configString,
                    ParentResourceId = parentResourceId != 0 ? parentResourceId : default(int?),
                    Operations = new List<CloudResourceOperation> {
                    new CloudResourceOperation()
                    {
                    Description = AzureResourceUtil.CreateDescriptionForSandboxResourceOperation(type, CloudResourceOperationType.CREATE, sandboxId),
                    BatchId = batchId,
                    OperationType = CloudResourceOperationType.CREATE,
                    CreatedBy = currentUser.UserName,
                    CreatedBySessionId = _requestIdService.GetRequestId(),
                    DependsOnOperationId = operationDependsOn != 0 ? operationDependsOn: default(int?),
                    MaxTryCount = CloudResourceConstants.RESOURCE_MAX_TRY_COUNT
                    }
                },
                    CreatedBy = currentUser.UserName,
                    Created = DateTime.UtcNow
                };

                sandboxFromDb.Resources.Add(newResource);

                await _db.SaveChangesAsync();

                return newResource;

            }
            catch (Exception ex)
            {

                throw;
            }
        }
    }
}
