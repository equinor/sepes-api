using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Dto.Sandbox;
using Sepes.Infrastructure.Dto.VirtualMachine;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Query;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Service.Queries;
using Sepes.Infrastructure.Util;
using Sepes.Infrastructure.Util.Provisioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class VirtualMachineCreateService : VirtualMachineServiceBase, IVirtualMachineCreateService
    {
         
        readonly ISandboxModelService _sandboxModelService;
        readonly IVirtualMachineSizeService _vmSizeService;
        readonly IVirtualMachineOperatingSystemService _virtualMachineOperatingSystemService;
        readonly ICloudResourceReadService _sandboxResourceService;
        readonly ICloudResourceCreateService _sandboxResourceCreateService;
        readonly ICloudResourceUpdateService _sandboxResourceUpdateService;
        readonly ICloudResourceDeleteService _sandboxResourceDeleteService;
        readonly IProvisioningQueueService _workQueue;
        readonly IAzureVirtualMachineExtenedInfoService _azureVirtualMachineExtenedInfoService;

        public VirtualMachineCreateService(
              IConfiguration config,
            SepesDbContext db,
            ILogger<VirtualMachineCreateService> logger,          
            IMapper mapper,
            IUserService userService,   
            

            ISandboxModelService sandboxModelService,
            IVirtualMachineSizeService vmSizeService,
            IVirtualMachineOperatingSystemService virtualMachineOperatingSystemService,
            ICloudResourceCreateService sandboxResourceCreateService,
            ICloudResourceUpdateService sandboxResourceUpdateService,
            ICloudResourceDeleteService sandboxResourceDeleteService,
            ICloudResourceReadService sandboxResourceService,
            IProvisioningQueueService workQueue,
            IAzureVirtualMachineExtenedInfoService azureVirtualMachineExtenedInfoService)
           :base (config, db, logger, mapper, userService)
        {
           
                 
            _sandboxModelService = sandboxModelService;
            _vmSizeService = vmSizeService;
            _sandboxResourceService = sandboxResourceService;
            _virtualMachineOperatingSystemService = virtualMachineOperatingSystemService;
            _sandboxResourceCreateService = sandboxResourceCreateService;
            _sandboxResourceUpdateService = sandboxResourceUpdateService;
            _sandboxResourceDeleteService = sandboxResourceDeleteService;
            _workQueue = workQueue;
            _azureVirtualMachineExtenedInfoService = azureVirtualMachineExtenedInfoService;
        }

        public async Task<VmDto> CreateAsync(int sandboxId, VirtualMachineCreateDto userInput)
        {
            CloudResource vmResourceEntry = null;

            try
            {
                ValidateVmPasswordOrThrow(userInput.Password);

                GenericNameValidation.ValidateName(userInput.Name);

                _logger.LogInformation($"Creating Virtual Machine for sandbox: {sandboxId}");

                var sandbox = await _sandboxModelService.GetByIdAsync(sandboxId, UserOperation.Study_Crud_Sandbox, true);

                var virtualMachineName = AzureResourceNameUtil.VirtualMachine(sandbox.Study.Name, sandbox.Name, userInput.Name);

                await _sandboxResourceCreateService.ValidateThatNameDoesNotExistThrowIfInvalid(virtualMachineName);

                var tags = AzureResourceTagsFactory.SandboxResourceTags(_config, sandbox.Study, sandbox);

                var region = RegionStringConverter.Convert(sandbox.Region);

                userInput.DataDisks = await TranslateDiskSizes(sandbox.Region, userInput.DataDisks);

                var resourceGroup = await CloudResourceQueries.GetResourceGroupEntry(_db, sandboxId);

                //Make this dependent on bastion create operation to be completed, since bastion finishes last
                var dependsOn = await CloudResourceQueries.GetCreateOperationIdForBastion(_db, sandboxId);

                vmResourceEntry = await _sandboxResourceCreateService.CreateVmEntryAsync(sandboxId, resourceGroup, region.Name, tags, virtualMachineName, dependsOn, null);

                //Create vm settings and immeately attach to resource entry
                var vmSettingsString = await CreateVmSettingsString(sandbox.Region, vmResourceEntry.Id, sandbox.Study.Id, sandboxId, userInput);
                vmResourceEntry.ConfigString = vmSettingsString;
                await _sandboxResourceUpdateService.Update(vmResourceEntry.Id, vmResourceEntry);

                var queueParentItem = new ProvisioningQueueParentDto
                {                    
                    Description = $"Create VM for Sandbox: {sandboxId}"
                };

                queueParentItem.Children.Add(new ProvisioningQueueChildDto() { ResourceOperationId = vmResourceEntry.Operations.FirstOrDefault().Id });

                await _workQueue.SendMessageAsync(queueParentItem);

                var dtoMappedFromResource = _mapper.Map<VmDto>(vmResourceEntry);

                return dtoMappedFromResource;
            }
            catch (Exception ex)
            {
                try
                {
                    //Delete resource if created
                    if (vmResourceEntry != null)
                    {
                        await _sandboxResourceDeleteService.HardDeletedAsync(vmResourceEntry.Id);
                    }
                }
                catch (Exception rollbackEx)
                {
                    _logger.LogError(rollbackEx, $"Failed to roll back VM creation for sandbox {sandboxId}");
                }

                throw new Exception($"Failed to create VM: {ex.Message}", ex);
            }
        }

        async Task<List<string>> TranslateDiskSizes(string region, List<string> dataDisksFromClient)
        {
            //Fix disks
            var disksFromDbForGivenRegion = await _db.RegionDiskSize.Where(rds => rds.RegionKey == region).Select(rds => rds.DiskSize).ToDictionaryAsync(ds => ds.Key, ds => ds);

            if (disksFromDbForGivenRegion.Count == 0)
            {
                throw new Exception($"No data disk items found in DB");
            }

            var result = new List<string>();

            foreach (var curDataDisk in dataDisksFromClient)
            {
                if (disksFromDbForGivenRegion.TryGetValue(curDataDisk, out DiskSize diskSize))
                {
                    result.Add(Convert.ToString(diskSize.Size));
                }
                else
                {
                    throw new Exception($"Unknown data disk size specification: {curDataDisk}");
                }
            }

            return result;
        }          
    }
}
