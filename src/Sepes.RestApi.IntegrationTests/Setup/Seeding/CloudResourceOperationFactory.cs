using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Model;
using System;

namespace Sepes.RestApi.IntegrationTests.Setup.Seeding
{
    public static class CloudResourceOperationFactory
    {  
        public static CloudResourceOperation NewOperation(string description, string operationType = CloudResourceOperationType.CREATE, string batchId = null)
        {
            var operation = BasicOperation(description, operationType, batchId: batchId);
            return operation;
        }

        public static CloudResourceOperation SucceededOperation(string description, string operationType = CloudResourceOperationType.CREATE, string batchId = null)
        {
            var operation = BasicOperation(description, operationType, status: CloudResourceOperationState.DONE_SUCCESSFUL, batchId: batchId);
            return operation;
        }

        public static CloudResourceOperation FailedOperation(string description, string operationType = CloudResourceOperationType.CREATE, string batchId = null, string status = CloudResourceOperationState.FAILED, int tryCount = 5, int maxTryCount = 5)
        {
            var operation = BasicOperation(description, operationType, status: status, batchId: batchId, tryCount, maxTryCount);
            return operation;
        }

        //public static CloudResourceOperation AbortedOperation(string description, string operationType = CloudResourceOperationType.CREATE, string batchId = null, int tryCount = 5, int maxTryCount = 5)
        //{
        //    var operation = FailedOperation(description, operationType, batchId: batchId, status: CloudResourceOperationState.ABORTED, tryCount, maxTryCount);
        //    return operation;
        //}

        //public static CloudResourceOperation AbandonedOperation(string description, string operationType = CloudResourceOperationType.CREATE, string batchId = null, int tryCount = 5, int maxTryCount = 5)
        //{
        //    var operation = FailedOperation(description, operationType, batchId: batchId, status: CloudResourceOperationState.ABANDONED, tryCount, maxTryCount);
        //    return operation;
        //}

        public static CloudResourceOperation BasicOperation(string description, string operationType, string status = CloudResourceOperationState.NEW, string batchId = null, int tryCount = 0, int maxTryCount = CloudResourceConstants.RESOURCE_MAX_TRY_COUNT)
        {
            return new CloudResourceOperation()
            {
                CreatedBySessionId = Guid.NewGuid().ToString(),
                Description = description,               
                OperationType = operationType,
                Status = status,
                BatchId = batchId,
                CreatedBy = "seed",
                Created = DateTime.UtcNow,
                UpdatedBy = "seed",
                Updated = DateTime.UtcNow,
                TryCount = tryCount,
                MaxTryCount = maxTryCount,
            };
        }
    }
}
