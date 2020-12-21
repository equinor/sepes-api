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

        public async Task CreateSandboxResourceGroup(SandboxResourceCreationAndSchedulingDto dto)
        {
            _logger.LogInformation($"Creating Resource Group database entry for sandbox: {dto.SandboxId}");

            var resourceGroupName = AzureResourceNameUtil.SandboxResourceGroup(dto.StudyName, dto.SandboxName);
            var resourceGroupEntry = await AddInternal(dto.BatchId, dto.SandboxId, "not created", resourceGroupName, AzureResourceType.ResourceGroup, dto.Region.Name, resourceGroupName, dto.Tags);
            dto.ResourceGroup = MapEntityToDto(resourceGroupEntry);
            //var resourceCreateOperation = resourceEntity.Operations.FirstOrDefault();
            //await _sandboxResourceOperationService.SetInProgressAsync(resourceCreateOperation.Id, _requestIdService.GetRequestId(), CloudResourceOperationState.IN_PROGRESS);

            //dto.ResourceGroup = MapEntityToDto(resourceEntity);

            //var azureResourceGroup = await _resourceGroupService.Create(resourceEntity.ResourceName, dto.Region, dto.Tags);
            //ApplyPropertiesFromResourceGroup(azureResourceGroup, dto.ResourceGroup);

            //_ = await UpdateResourceGroup(dto.ResourceGroup.Id, dto.ResourceGroup);
            //_ = await _sandboxResourceOperationService.UpdateStatusAsync(dto.ResourceGroup.Operations.FirstOrDefault().Id, CloudResourceOperationState.DONE_SUCCESSFUL);
        }

        public async Task<CloudResourceDto> CreateVmEntryAsync(int sandboxId, CloudResource resourceGroup, Microsoft.Azure.Management.ResourceManager.Fluent.Core.Region region, Dictionary<string, string> tags, string vmName, int dependsOn, string configString)
        {
            try
            {
                var resourceEntity = await AddInternal(Guid.NewGuid().ToString(),
                    sandboxId,
                    resourceGroup.ResourceGroupId, resourceGroup.ResourceGroupName, AzureResourceType.VirtualMachine, region.Name, vmName, tags, false, dependentOn: dependsOn, configString: configString);

                return MapEntityToDto(resourceEntity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to create database resource entry for Virtual Machine for Sandbox {sandboxId}. See inner Exception for details", ex);
            }
        }

        public async Task<CloudResourceDto> Create(SandboxResourceCreationAndSchedulingDto dto, string type, string resourceName, bool sandboxControlled = true, string configString = null, int dependsOn = 0)
        {
            var newResource = await AddInternal(dto.BatchId, dto.SandboxId, dto.ResourceGroupId, dto.ResourceGroupName, type, dto.Region.Name, resourceName, dto.Tags, sandboxControlled: sandboxControlled, dependentOn: dependsOn, configString: configString);

            var mappedToDto = MapEntityToDto(newResource);

            return mappedToDto;
        }

        public async Task ValidateNameThrowIfInvalid(string resourceName)
        {
            if(await _db.CloudResources.Where(r=> r.ResourceName == resourceName && !r.Deleted.HasValue).AnyAsync())
            {
                throw new Exception($"Resource with name {resourceName} allready exists!");
            }
        }

        async Task<CloudResource> AddInternal(string batchId, int sandboxId, string resourceGroupId, string resourceGroupName, string type, string region, string resourceName, Dictionary<string, string> tags, bool sandboxControlled = true, int dependentOn = 0, string configString = null)
        {
           await ValidateNameThrowIfInvalid(resourceName);

            var sandboxFromDb = await GetSandboxOrThrowAsync(sandboxId);

            var tagsString = AzureResourceTagsFactory.TagDictionaryToString(tags);

            var currentUser = await _userService.GetCurrentUserAsync();

            var newResource = new CloudResource()
            {
                ResourceGroupId = resourceGroupId,
                ResourceGroupName = resourceGroupName,
                ResourceType = type,
                ResourceKey = AzureResourceNameUtil.AZURE_RESOURCE_INITIAL_ID_OR_NAME,
                ResourceName = resourceName,
                ResourceId = AzureResourceNameUtil.AZURE_RESOURCE_INITIAL_ID_OR_NAME,
                SandboxControlled = sandboxControlled,
                Region = region,
                Tags = tagsString,
                ConfigString = configString,

                Operations = new List<CloudResourceOperation> {
                    new CloudResourceOperation()
                    {
                    Description = AzureResourceUtil.CreateDescriptionForResourceOperation(type, CloudResourceOperationType.CREATE, sandboxId),
                    BatchId = batchId,
                    OperationType = CloudResourceOperationType.CREATE,
                    CreatedBy = currentUser.UserName,
                    CreatedBySessionId = _requestIdService.GetRequestId(),
                    DependsOnOperationId = dependentOn != 0 ? dependentOn: default(int?),
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
    }
}
