using Sepes.Azure.Service.Interface;
using Sepes.Common.Constants.CloudResource;
using Sepes.Common.Dto;
using Sepes.Common.Dto.Sandbox;
using Sepes.Common.Exceptions;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Provisioning.Service.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Provisioning.Service
{
    public class TagProvisioningService : ITagProvisioningService
    {
        readonly IProvisioningLogService _provisioningLogService;
        readonly ICloudResourceReadService _cloudResourceReadService;
        readonly ICloudResourceOperationUpdateService _cloudResourceOperationUpdateService;

        public TagProvisioningService(IProvisioningLogService provisioningLogService, ICloudResourceReadService cloudResourceReadService,
            ICloudResourceOperationUpdateService cloudResourceOperationUpdateService)
        {
            _provisioningLogService = provisioningLogService ?? throw new ArgumentNullException(nameof(provisioningLogService));
            _cloudResourceReadService = cloudResourceReadService ??
                                        throw new ArgumentNullException(nameof(cloudResourceReadService));
            _cloudResourceOperationUpdateService = cloudResourceOperationUpdateService ??
                                                   throw new ArgumentNullException(
                                                       nameof(cloudResourceOperationUpdateService));
        }
        public bool CanHandle(CloudResourceOperationDto operation)
        {
            return operation.OperationType == CloudResourceOperationType.ENSURE_TAGS;
        }

        public async Task Handle(ProvisioningQueueParentDto queueParentItem, CloudResourceOperationDto operation, IServiceForTaggedResource tagService)
        {
            try
            {
                using (var cancellation = new CancellationTokenSource())
                {
                    var setRulesTask = tagService.SetTagsAsync(operation.Resource.ResourceGroupName, operation.Resource.ResourceName, operation.Resource.Tags, cancellation.Token);

                    while (!setRulesTask.IsCompleted)
                    {
                        operation = await _cloudResourceOperationUpdateService.TouchAsync(operation.Id);

                        if (await _cloudResourceReadService.ResourceIsDeleted(operation.Resource.Id) || operation.Status == CloudResourceOperationState.ABORTED || operation.Status == CloudResourceOperationState.ABANDONED)
                        {
                            _provisioningLogService.OperationWarning(queueParentItem, operation, $"Operation aborted, ensuring tags task will be aborted");
                            cancellation.Cancel();
                            break;
                        }

                        Thread.Sleep((int)TimeSpan.FromSeconds(3).TotalMilliseconds);
                    }

                    if (setRulesTask.IsCompletedSuccessfully)
                    {

                    }
                    else
                    {
                        if (setRulesTask.Exception == null)
                        {
                            throw new Exception("ensuring tags task failed");
                        }
                        else
                        {
                            throw setRulesTask.Exception;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("A task was canceled"))
                {
                    throw new ProvisioningException($"Resource provisioning (Ensure tags) aborted.", logAsWarning: true, innerException: ex.InnerException);
                }
                else
                {
                    throw new ProvisioningException($"Resource provisioning (Ensure tags) failed.", CloudResourceOperationState.FAILED, postponeQueueItemFor: 10, innerException: ex);
                }
            }
        }
    }
}
