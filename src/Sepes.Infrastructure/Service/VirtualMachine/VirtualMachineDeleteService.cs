using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Model.Context;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util.Provisioning;
using System.Threading.Tasks;

namespace Sepes.Infrastructure.Service
{
    public class VirtualMachineDeleteService : VirtualMachineServiceBase, IVirtualMachineDeleteService
    {
        readonly IProvisioningQueueService _provisioningQueueService;
        readonly ICloudResourceDeleteService _sandboxResourceDeleteService;
         

        public VirtualMachineDeleteService(
            IConfiguration config,
            SepesDbContext db, 
            ILogger<VirtualMachineDeleteService> logger,         
            IMapper mapper,
            IUserService userService,
            IProvisioningQueueService provisioningQueueService,
            ICloudResourceReadService cloudResourceReadService
          
          )
             : base(config, db, logger, mapper, userService, cloudResourceReadService)
        {
            _provisioningQueueService = provisioningQueueService;         
        }      

        public async Task DeleteAsync(int id)
        {
            var deleteResourceOperation = await _sandboxResourceDeleteService.MarkAsDeletedWithDeleteOperationAsync(id, UserOperation.Study_Crud_Sandbox);

            _logger.LogInformation($"Delete VM: Enqueing delete operation");

           var queueParentItem = QueueItemFactory.CreateParent(deleteResourceOperation);           

            await _provisioningQueueService.SendMessageAsync(queueParentItem);
        }       
    }
}
