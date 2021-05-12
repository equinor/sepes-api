using System;
using Microsoft.Extensions.Logging;
using Sepes.Common.Dto;
using Sepes.Common.Dto.Sandbox;
using Sepes.Provisioning.Service.Interface;

namespace Sepes.Provisioning.Service
{
    public class ProvisioningLogService : IProvisioningLogService
    {
        readonly ILogger _logger;

        public ProvisioningLogService(ILogger<ProvisioningLogService> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void HandlingQueueParent(ProvisioningQueueParentDto queueParentItem)
        {
            _logger.LogInformation($"Handling: {queueParentItem.MessageId} - {queueParentItem.Description}");
        }

        public void QueueParentProgressInformation(ProvisioningQueueParentDto queueParentItem, string suffix)
        {
            _logger.LogInformation(QueueParentProgressLogMessage(queueParentItem, suffix));
        }
        
        public void QueueParentProgressWarning(ProvisioningQueueParentDto queueParentItem, string suffix)
        {
            _logger.LogWarning(QueueParentProgressLogMessage(queueParentItem, suffix));
        }
        
        public void QueueParentProgressError(ProvisioningQueueParentDto queueParentItem, string suffix, Exception exeption = null)
        {
            _logger.LogError(exeption, QueueParentProgressLogMessage(queueParentItem, suffix));
        }
        
        public void QueueParentProgressCritical(ProvisioningQueueParentDto queueParentItem, string suffix)
        {
            _logger.LogCritical(QueueParentProgressLogMessage(queueParentItem, suffix));
        }

        string QueueParentProgressLogMessage(ProvisioningQueueParentDto queueParentItem, string suffix)
        {
            return $"MessageId: {queueParentItem.MessageId} - {queueParentItem.Description} | {suffix}";
        }

        public void OperationInformation(CloudResourceOperationDto currentResourceOperation, string suffix)
        {
            _logger.LogInformation(CurrentOperationLogMessage(currentResourceOperation, suffix));
        }
        
        public void OperationWarning(CloudResourceOperationDto currentResourceOperation, string suffix, Exception exeption = null)
        {
            _logger.LogWarning(exeption, CurrentOperationLogMessage(currentResourceOperation, suffix));
        }
        
        public void OperationError(Exception exeption, CloudResourceOperationDto currentResourceOperation, string suffix)
        {
            _logger.LogError(exeption, CurrentOperationLogMessage(currentResourceOperation, suffix));
        }

        string CurrentOperationLogMessage(CloudResourceOperationDto currentResourceOperation, string suffix)
        {
            return $"{currentResourceOperation.Resource.SandboxName} | {currentResourceOperation.Id} | {currentResourceOperation.Resource.ResourceType} | {currentResourceOperation.OperationType.ToUpper()} | attempt ({currentResourceOperation.TryCount}/{currentResourceOperation.MaxTryCount}) | {suffix}";
        }
    }
}
