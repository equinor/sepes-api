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
            _logger.LogInformation($"{queueParentItem.MessageId} - {queueParentItem.Description}: Message being handled");
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
            return $"{queueParentItem.MessageId} - {queueParentItem.Description} | {suffix}";
        }

        public void OperationInformation(ProvisioningQueueParentDto queueParentItem, CloudResourceOperationDto currentResourceOperation, string suffix, EventId eventId = default(EventId))
        {
            _logger.LogInformation(eventId, CurrentOperationLogMessage(queueParentItem, currentResourceOperation, suffix));
        }
        
        public void OperationWarning(ProvisioningQueueParentDto queueParentItem, CloudResourceOperationDto currentResourceOperation, string suffix, Exception exeption = null, EventId eventId = default(EventId))
        {
            _logger.LogWarning(eventId, exeption, CurrentOperationLogMessage(queueParentItem, currentResourceOperation, suffix));
        }
        
        public void OperationError(ProvisioningQueueParentDto queueParentItem, Exception exeption, CloudResourceOperationDto currentResourceOperation, string suffix)
        {
            _logger.LogError(exeption, CurrentOperationLogMessage(queueParentItem, currentResourceOperation, suffix));
        }

        string CurrentOperationLogMessage(ProvisioningQueueParentDto queueParentItem, CloudResourceOperationDto currentResourceOperation, string suffix)
        {
            var sandboxPart = String.IsNullOrWhiteSpace(currentResourceOperation.Resource.SandboxName) ? "" : $" {currentResourceOperation.Resource.SandboxName} | ";
            return $"{queueParentItem.MessageId} | {currentResourceOperation.Id} |{sandboxPart} {currentResourceOperation.Resource.ResourceType} | {currentResourceOperation.OperationType.ToUpper()} | {currentResourceOperation.TryCount}/{currentResourceOperation.MaxTryCount} | {suffix}";
        }
    }
}
