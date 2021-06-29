using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Constants;
using Sepes.Common.Dto.VirtualMachine;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Query;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using Sepes.Infrastructure.Util.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class VirtualMachineReadService : VirtualMachineServiceBase, IVirtualMachineReadService
    {
        readonly IStudyPermissionService _studyPermissionService;
        readonly IVirtualMachineSizeService _virtualMachineSizeService;
        readonly IAzureVirtualMachineExtendedInfoService _azureVirtualMachineExtenedInfoService;

        public VirtualMachineReadService(
            IConfiguration config,
            SepesDbContext db,
            ILogger<VirtualMachineReadService> logger,
            IMapper mapper,
            IUserService userService,
            IStudyPermissionService studyPermissionService,
            ICloudResourceReadService cloudResourceReadService,
            IVirtualMachineSizeService virtualMachineSizeService,
            IAzureVirtualMachineExtendedInfoService azureVirtualMachineExtenedInfoService

          )
           : base(config, db, logger, mapper, userService, cloudResourceReadService)
        {
            _studyPermissionService = studyPermissionService;
            _virtualMachineSizeService = virtualMachineSizeService;             
            _azureVirtualMachineExtenedInfoService = azureVirtualMachineExtenedInfoService;
        }

        public async Task<List<VmDto>> VirtualMachinesForSandboxAsync(int sandboxId, CancellationToken cancellationToken = default)
        {
            var virtualMachines = await GetSandboxVirtualMachinesList(_db, sandboxId);

            var virtualMachinesMapped = _mapper.Map<List<VmDto>>(virtualMachines);

            return virtualMachinesMapped;
        }

        public async Task<VmExtendedDto> GetExtendedInfo(int vmId, CancellationToken cancellationToken = default)
        {
            var vmResource = await GetVirtualMachineResourceEntry(vmId, UserOperation.Study_Read, cancellationToken);           

            var dto = await _azureVirtualMachineExtenedInfoService.GetExtendedInfo(vmResource.ResourceGroupName, vmResource.ResourceName);

            var availableSizes = await _virtualMachineSizeService.AvailableSizes(vmResource.Region, cancellationToken);

            var availableSizesDict = availableSizes.ToDictionary(s => s.Key, s => s);

            if (!String.IsNullOrWhiteSpace(dto.SizeName) && availableSizesDict.TryGetValue(dto.SizeName, out VmSize curSize))
            {
                dto.Size = _mapper.Map<VmSizeDto>(curSize);
            }

            return dto;
        }

        public async Task<VmExternalLink> GetExternalLink(int vmId, CancellationToken cancellationToken = default)
        {
            var vmResource = await GetVirtualMachineResourceEntry(vmId, UserOperation.Study_Read, cancellationToken);

            var vmExternalLink = new VmExternalLink
            {
                Id = vmId,
                LinkToExternalSystem = CloudResourceUtil.CreateResourceLink(_config, vmResource)
            };

            return vmExternalLink;
        }

        async Task<List<CloudResource>> GetSandboxVirtualMachinesList(SepesDbContext db, int sandboxId)
        {
            var queryable = CloudResourceQueries.SandboxVirtualMachinesQueryable(db, sandboxId);    
            
            var vmList = await queryable.ToListAsync();

            if (!vmList.Any())
            {
                return new List<CloudResource>();
            }

            var sandbox = vmList.FirstOrDefault().Sandbox;

            await _studyPermissionService.VerifyAccessOrThrow(sandbox.Study, UserOperation.Study_Read);
            
            return vmList;
        }
    }
}
