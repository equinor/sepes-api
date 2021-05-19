using Microsoft.Extensions.Logging;
using Sepes.Azure.Service.Interface;
using Sepes.Common.Constants;
using Sepes.Common.Constants.Auth;
using Sepes.Common.Constants.CloudResource;
using Sepes.Common.Dto;
using Sepes.Common.Exceptions;
using Sepes.Common.Util;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Service.DataModelService.Interface;
using Sepes.Infrastructure.Service.Interface;
using Sepes.Infrastructure.Util.Auth;
using Sepes.Provisioning.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Sepes.Provisioning.Service
{
    public class RoleProvisioningService : IRoleProvisioningService
    {
        readonly IProvisioningLogService _provisioningLogService;
        readonly IStudyModelService _studyModelService;
        readonly ICloudResourceReadService _cloudResourceReadService;
        readonly ICloudResourceOperationUpdateService _cloudResourceOperationUpdateService;
        readonly IAzureRoleAssignmentService _azureRoleAssignmentService;

        readonly EventId _roleAssignmentEventId = new EventId(50, "Sepes-Event-RoleAssignment-Operations");

        public RoleProvisioningService(IProvisioningLogService provisioningLogService, IStudyModelService studyModelService, ICloudResourceReadService cloudResourceReadService, ICloudResourceOperationUpdateService cloudResourceOperationUpdateService, IAzureRoleAssignmentService roleAssignmentService)
        {
            _provisioningLogService = provisioningLogService;
            _studyModelService = studyModelService;
            _cloudResourceReadService = cloudResourceReadService;
            _cloudResourceOperationUpdateService = cloudResourceOperationUpdateService;
            _azureRoleAssignmentService = roleAssignmentService;
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

                var operationStateDeserialized = CloudResourceConfigStringSerializer.DesiredRoleAssignment(operation.DesiredState);
                var study = await _studyModelService.GetWithParticipantsAndUsersNoAccessCheck(operationStateDeserialized.StudyId);
                var currentRoleAssignmentTask = SetRoleAssignments(operation, study, cancellation.Token);

                while (!currentRoleAssignmentTask.IsCompleted)
                {
                    operation = await _cloudResourceOperationUpdateService.TouchAsync(operation.Id);

                    if (await _cloudResourceReadService.ResourceIsDeleted(operation.Resource.Id)
                        || operation.Status == CloudResourceOperationState.ABORTED
                        || operation.Status == CloudResourceOperationState.ABANDONED)
                    {
                        _provisioningLogService.OperationWarning(operation, $"Operation aborted, role assignment will be aborted", eventId: _roleAssignmentEventId);
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

        async Task SetRoleAssignments(CloudResourceOperationDto operation, Study study, CancellationToken cancellationToken = default)
        {           
            _provisioningLogService.OperationInformation(operation, "SetRoleAssignments", eventId: _roleAssignmentEventId);           

            List<CloudResourceDesiredRoleAssignmentDto> desiredRoleAssignments = null;
            
            if(operation.Resource.Purpose == CloudResourcePurpose.SandboxResourceGroup)
            {
                desiredRoleAssignments = ParticipantRoleToAzureRoleTranslator.CreateDesiredRolesForSandboxResourceGroup(study.StudyParticipants.ToList());
            }
            else if (operation.Resource.Purpose == CloudResourcePurpose.StudySpecificDatasetContainer)
            {
                desiredRoleAssignments = ParticipantRoleToAzureRoleTranslator.CreateDesiredRolesForStudyResourceGroup(study.StudyParticipants.ToList());
            }               
           
            var existingRoleAssignments = await _azureRoleAssignmentService.GetResourceGroupRoleAssignments(operation.Resource.ResourceId, operation.Resource.ResourceName, cancellationToken);
            
            //Create desired roles that does not allready exist
            foreach (var curDesired in desiredRoleAssignments)
            {
                var sameRoleFromExisting = existingRoleAssignments.Where(ra => ra.properties.principalId == curDesired.PrincipalId && ra.properties.roleDefinitionId.Contains(curDesired.RoleId)).FirstOrDefault();

                if (sameRoleFromExisting != null)
                {
                    _provisioningLogService.OperationInformation(operation, $"Principal {curDesired.PrincipalId} allready had role {curDesired.RoleId}", eventId: _roleAssignmentEventId);
                }
                else
                {
                    var roleDefinitionId = AzureRoleIds.CreateRoleDefinitionUrl(operation.Resource.ResourceId, curDesired.RoleId);
                    _provisioningLogService.OperationInformation(operation, $"Principal {curDesired.PrincipalId} missing role {curDesired.RoleId}, creating. Role definition: {roleDefinitionId}", eventId: _roleAssignmentEventId);
                    await _azureRoleAssignmentService.AddRoleAssignment(operation.Resource.ResourceId, roleDefinitionId, curDesired.PrincipalId, cancellationToken: cancellationToken);
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
                    _provisioningLogService.OperationInformation(operation, $"Existing role for principal {curExisting.properties.principalId} with id {curExisting.properties.roleDefinitionId} also in desired role list. Keeping", eventId: _roleAssignmentEventId);
                }
                else
                {
                    _provisioningLogService.OperationInformation(operation, $"Existing role for principal {curExisting.properties.principalId} with id {curExisting.properties.roleDefinitionId} NOT in desired role list. Will be deleted", eventId: _roleAssignmentEventId);
                    await _azureRoleAssignmentService.DeleteRoleAssignment(curExisting.id, cancellationToken);
                }
            }
        }

    }
}
