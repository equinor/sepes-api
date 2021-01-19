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

        public async Task<CloudResourceDto> Create(SandboxResourceCreationAndSchedulingDto dto, string type, string resourceName, bool sandboxControlled = true, string configString = null, int operationDependsOn = 0)
        {
            _logger.LogInformation($"Creating Resource entry for sandbox: {dto.SandboxId}, resource type: {type}");

            var newResource = await AddInternal(dto.SandboxId, type, dto.Region.Name, dto.ResourceGroupName, resourceName, dto.Tags, dto.ResourceGroup != null ? dto.ResourceGroup.Id : 0, sandboxControlled: sandboxControlled, operationDependsOn: operationDependsOn, configString: configString, batchId: dto.BatchId);

            var mappedToDto = MapEntityToDto(newResource);

            return mappedToDto;
        }

        public async Task<CloudResourceDto> CreateSandboxResourceGroupEntryAsync(SandboxResourceCreationAndSchedulingDto dto, string resourceGroupName)
        {
            var resourceGroupEntry = await AddInternal(dto.SandboxId, AzureResourceType.ResourceGroup, dto.Region.Name, resourceGroupName, resourceGroupName, dto.Tags, batchId: dto.BatchId);
            return MapEntityToDto(resourceGroupEntry);
        }

        public async Task<CloudResourceDto> CreateVmEntryAsync(int sandboxId, CloudResource resourceGroup, Microsoft.Azure.Management.ResourceManager.Fluent.Core.Region region, Dictionary<string, string> tags, string vmName, int operationDependsOn, string configString)
        {
            try
            {
                var resourceEntity = await AddInternal(
                    sandboxId, AzureResourceType.VirtualMachine, region.Name, resourceGroup.ResourceGroupName, vmName, tags,
                    resourceGroup.Id, false, operationDependsOn: operationDependsOn, configString: configString);

                return MapEntityToDto(resourceEntity);
            }
            catch (Exception ex)
            {
                throw new Exception($"Unable to create database resource entry for Virtual Machine for Sandbox {sandboxId}. See inner Exception for details", ex);
            }
        }

        public async Task ValidateNameThrowIfInvalid(string resourceName)
        {
            if (await _db.CloudResources.Where(r => r.ResourceName == resourceName && !r.Deleted.HasValue).AnyAsync())
            {
                throw new Exception($"Resource with name {resourceName} allready exists!");
            }
        }

        async Task<CloudResource> AddInternal(int sandboxId, string type, string region, string resourceGroupName, string resourceName, Dictionary<string, string> tags, int? parentResourceId = default, bool sandboxControlled = true, int? operationDependsOn = default, string configString = null, string batchId = null)
        {
            try
            {
                await ValidateNameThrowIfInvalid(resourceName);

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
                    Description = AzureResourceUtil.CreateDescriptionForResourceOperation(type, CloudResourceOperationType.CREATE, sandboxId),
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
