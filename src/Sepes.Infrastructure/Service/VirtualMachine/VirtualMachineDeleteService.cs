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
    public class VirtualMachineDeleteService : VirtualMachineServiceBase, IVirtualMachineDeleteService
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

        public VirtualMachineDeleteService(ILogger<VirtualMachineDeleteService> logger,
            IConfiguration config,
            SepesDbContext db,
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
             : base(config, db, logger, mapper, userService)
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

        public async Task DeleteAsync(int id)
        {
            var vmResource = await GetVmResourceEntry(id, UserOperation.Study_Crud_Sandbox);

            var deleteResourceOperation = await _sandboxResourceDeleteService.MarkAsDeletedWithDeleteOperationAsync(id);

            _logger.LogInformation($"Delete VM: Enqueing delete operation");

           var queueParentItem = QueueItemFactory.CreateParent(deleteResourceOperation);           

            await _workQueue.SendMessageAsync(queueParentItem);
        }       
    }
}
