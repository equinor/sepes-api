using Sepes.Azure.Service.Interface;
using Sepes.Common.Constants.CloudResource;
using Sepes.Common.Dto;
using Sepes.Common.Dto.Sandbox;
using Sepes.Common.Exceptions;
using Sepes.Common.Util;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Provisioning.Service.Interface;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Provisioning.Service
{
    public class CorsRuleProvisioningService : ICorsRuleProvisioningService
    {
        readonly IProvisioningLogService _provisioningLogService;
        readonly ICloudResourceReadService _cloudResourceReadService;
        readonly ICloudResourceOperationUpdateService _cloudResourceOperationUpdateService;

        public CorsRuleProvisioningService(IProvisioningLogService provisioningLogService, ICloudResourceReadService cloudResourceReadService,
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
            return operation.OperationType == CloudResourceOperationType.ENSURE_CORS_RULES;
        }

        public async Task Handle(
             ProvisioningQueueParentDto queueParentItem,
            CloudResourceOperationDto operation,
            IHasCorsRules corsRuleService)
        {
            try
            {
                using (var cancellation = new CancellationTokenSource())
                {
                    if (string.IsNullOrWhiteSpace(operation.DesiredState))
                    {
                        throw new NullReferenceException($"Desired state empty on operation {operation.Id}: {operation.Description}");
                    }

                    var rulesFromOperationState = CloudResourceConfigStringSerializer.DesiredCorsRules(operation.DesiredState);

                    var setRulesTask = corsRuleService.SetCorsRules(operation.Resource.ResourceGroupName, operation.Resource.ResourceName, rulesFromOperationState, cancellation.Token);

                    while (!setRulesTask.IsCompleted)
                    {
                        operation = await _cloudResourceOperationUpdateService.TouchAsync(operation.Id);

                        if (await _cloudResourceReadService.ResourceIsDeleted(operation.Resource.Id) || operation.Status == CloudResourceOperationState.ABORTED || operation.Status == CloudResourceOperationState.ABANDONED)
                        {
                            _provisioningLogService.OperationWarning(queueParentItem, operation, $"Operation aborted, cors rule assignment will be aborted");
                            cancellation.Cancel();
                            break;
                        }

                        Thread.Sleep((int)TimeSpan.FromSeconds(3).TotalMilliseconds);
                    }

                    if (!setRulesTask.IsCompletedSuccessfully)
                    {
                        if (setRulesTask.Exception == null)
                        {
                            throw new Exception("cors rule assignment task failed");
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
                    throw new ProvisioningException($"Resource provisioning (Ensure cors rules) aborted.", logAsWarning: true, innerException: ex.InnerException);
                }
                else
                {
                    throw new ProvisioningException($"Resource provisioning (Ensure cors rules) failed.", CloudResourceOperationState.FAILED, postponeQueueItemFor: 10, innerException: ex);
                }
            }
        }
    }
}
