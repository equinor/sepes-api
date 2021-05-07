using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.Auth;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Dto;
using Sepes.Infrastructure.Dto.Azure.RoleAssignment;
using Sepes.Infrastructure.Exceptions;
using Sepes.Infrastructure.Service.Azure.Interface;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util;
using Sepes.Infrastructure.Util.Provisioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sepes.Infrastructure.Service.Provisioning.Interface;

namespace Sepes.Infrastructure.Service.Provisioning
{
    public class RoleProvisioningService : IRoleProvisioningService
    {      
        readonly ILogger _logger;
        readonly ICloudResourceReadService _cloudResourceReadService;
        readonly ICloudResourceOperationUpdateService _cloudResourceOperationUpdateService;
        readonly IAzureRoleAssignmentService _roleAssignmentService;

        public RoleProvisioningService(ILogger<RoleProvisioningService> logger, IConfiguration config, ICloudResourceReadService cloudResourceReadService, ICloudResourceOperationUpdateService cloudResourceOperationUpdateService, IAzureRoleAssignmentService roleAssignmentService)
        {
            _logger = logger;
            _cloudResourceReadService = cloudResourceReadService;
            _cloudResourceOperationUpdateService = cloudResourceOperationUpdateService;
            _roleAssignmentService = roleAssignmentService;
        }

        public bool CanHandle(CloudResourceOperationDto operation)
        {
            return operation.OperationType == CloudResourceOperationType.ENSURE_ROLES;
        }

        public async Task Handle(CloudResourceOperationDto operation)
        {
            try
            {
                var cancellation = new CancellationTokenSource();

                if (string.IsNullOrWhiteSpace(operation.DesiredState))
                {
                    throw new NullReferenceException($"Desired state empty on operation {operation.Id}: {operation.Description}");
                }

                var desiredRolesFromOperation = CloudResourceConfigStringSerializer.DesiredRoleAssignment(operation.DesiredState);

                var currentRoleAssignmentTask = SetRoleAssignments(operation.Resource.ResourceId, operation.Resource.ResourceName, desiredRolesFromOperation, cancellation.Token);

                while (!currentRoleAssignmentTask.IsCompleted)
                {
                    operation = await _cloudResourceOperationUpdateService.TouchAsync(operation.Id);

                    if (await _cloudResourceReadService.ResourceIsDeleted(operation.Resource.Id)
                        || operation.Status == CloudResourceOperationState.ABORTED
                        || operation.Status == CloudResourceOperationState.ABANDONED)
                    {
                        _logger.LogWarning(ProvisioningLogUtil.Operation(operation, $"Operation aborted, role assignment will be aborted"));
                        cancellation.Cancel();
                        break;
                    }

                    Thread.Sleep((int)TimeSpan.FromSeconds(3).TotalMilliseconds);
                }

                if (!currentRoleAssignmentTask.IsCompletedSuccessfully)
                {
                    if (currentRoleAssignmentTask.Exception == null)
                    {
                        throw new Exception("Role assignment task failed");
                    }
                    else
                    {
                        throw currentRoleAssignmentTask.Exception;
                    }
                }               
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("A task was canceled"))
                {
                    throw new ProvisioningException($"Resource provisioning (Ensure role assignments) aborted.", logAsWarning: true, innerException: ex.InnerException);
                }
                else
                {
                    throw new ProvisioningException($"Resource provisioning (Ensure role assignments) failed.", CloudResourceOperationState.FAILED, postponeQueueItemFor: 10, innerException: ex);
                }
            }
        }

        async Task SetRoleAssignments(string resourceGroupId, string resourceGroupName, List<CloudResourceDesiredRoleAssignmentDto> desiredRoleAssignments, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"SetRoleAssignments");

            var existingRoleAssignments = await _roleAssignmentService.GetResourceGroupRoleAssignments(resourceGroupId, resourceGroupName, cancellationToken);

            //Create desired roles that does not allready exist
            foreach (var curDesired in desiredRoleAssignments)
            {
                var sameRoleFromExisting = existingRoleAssignments.Where(ra => ra.properties.principalId == curDesired.PrincipalId && ra.properties.roleDefinitionId.Contains(curDesired.RoleId)).FirstOrDefault();

                    if (sameRoleFromExisting != null)
                    {
                        _logger.LogInformation($"Principal {curDesired.PrincipalId} allready had role {curDesired.RoleId}");
                    }
                    else
                    {
                        var roleDefinitionId = AzureRoleIds.CreateRoleDefinitionUrl(resourceGroupId, curDesired.RoleId);
                        _logger.LogInformation($"Principal {curDesired.PrincipalId} missing role {curDesired.RoleId}, creating. Role definition: {roleDefinitionId}");
                        await _roleAssignmentService.AddRoleAssignment(resourceGroupId, roleDefinitionId, curDesired.PrincipalId, cancellationToken: cancellationToken);
                    }
            }

            //Find out what roles are allready in place, and delete those that are no longer needed
            foreach (var curExisting in existingRoleAssignments)
            {
                var curExistingRoleId = AzureRoleIds.GetRoleIdFromDefinition(curExisting.properties.roleDefinitionId);

                CloudResourceDesiredRoleAssignmentDto sameRoleFromDesired = null;

                if (curExistingRoleId != null)
                {
                    sameRoleFromDesired = desiredRoleAssignments.Where(ra => ra.PrincipalId == curExisting.properties.principalId && ra.RoleId == curExistingRoleId).FirstOrDefault();
                }

                if (sameRoleFromDesired != null)
                {
                    _logger.LogInformation($"Existing role for principal {curExisting.properties.principalId} with id {curExisting.properties.roleDefinitionId} also in desired role list. Keeping");
                }
                else
                {
                    _logger.LogInformation($"Existing role for principal {curExisting.properties.principalId} with id {curExisting.properties.roleDefinitionId} NOT in desired role list. Will be deleted");
                    await _roleAssignmentService.DeleteRoleAssignment(curExisting.id, cancellationToken);
                }
            }
        }
        
        string CreateRoleAssignmentErrorString(List<AzureRoleAssignment> azureRoleAssignments)
        {
            var sb = new StringBuilder();
            azureRoleAssignments.ForEach(ra => sb.AppendLine($"{ra.id} | {ra.properties.principalId} | {ra.properties.roleDefinitionId} "));
            return sb.ToString();
        }

        string CreateConfigErrorString(HashSet<string> filter)
        {
            var sb = new StringBuilder();

            foreach(var cur in filter)
            {
                sb.AppendLine(cur);
            }

            return sb.ToString();
        }

        
    }
}
