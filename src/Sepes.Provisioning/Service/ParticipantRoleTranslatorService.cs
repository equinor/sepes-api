using Sepes.Common.Constants;
using Sepes.Common.Constants.Auth;
using Sepes.Common.Dto;
using Sepes.Infrastructure.Model;
using Sepes.Provisioning.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Sepes.Common.Util;

namespace Sepes.Provisioning.Service
{
    public class ParticipantRoleTranslatorService : IParticipantRoleTranslatorService
    {
        readonly IConfiguration _configuration;

        public ParticipantRoleTranslatorService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public List<CloudResourceDesiredRoleAssignmentDto> CreateDesiredRolesForStudyDatasetResourceGroup(List<StudyParticipant> participants)
        {
            return CreateDesiredRolesWithTranslator(participants, TranslateForStudyDatasetResourceGroup);
        }

        public List<CloudResourceDesiredRoleAssignmentDto> CreateDesiredRolesForSandboxResourceGroup(List<StudyParticipant> participants)
        {
            return CreateDesiredRolesWithTranslator(participants, TranslateForSandboxResourceGroup);            
        }      

        List<CloudResourceDesiredRoleAssignmentDto> CreateDesiredRolesWithTranslator(List<StudyParticipant> participants, Func<string, string> translator)
        {
            var desiredRolesLookup = new Dictionary<Tuple<string, string>, CloudResourceDesiredRoleAssignmentDto>();

            foreach (var curParticipant in participants)
            {
                var translatedRole = translator(curParticipant.RoleName);

                if (!String.IsNullOrWhiteSpace(translatedRole))
                {
                    var lookupKey = CreateAssignmentLookupKey(curParticipant.User.ObjectId, translatedRole);

                    if (!desiredRolesLookup.ContainsKey(lookupKey))
                    {
                        desiredRolesLookup.Add(lookupKey, new CloudResourceDesiredRoleAssignmentDto(curParticipant.User.ObjectId, translatedRole));
                    }
                }
            }

            return desiredRolesLookup.Values.ToList();
        }

        Tuple<string, string> CreateAssignmentLookupKey(string principalId, string roleId)
        {
            return new Tuple<string, string>(principalId, roleId);
        }

        string TranslateForStudyDatasetResourceGroup(string studyParticipantRole)
        {
            var roleIdFromConfig = ConfigUtil.GetConfigValueAndThrowIfEmpty(_configuration,
                ConfigConstants.DATASET_STORAGEACCOUNT_ROLE_ASSIGNMENT_ID);
            
            return studyParticipantRole switch
            {
                StudyRoles.StudyOwner => roleIdFromConfig,
                StudyRoles.SponsorRep => roleIdFromConfig,
                _ => null,
            };
        }

        string TranslateForSandboxResourceGroup(string studyParticipantRole)
        {
            return studyParticipantRole switch
            {
                StudyRoles.StudyOwner => AzureRoleIds.CONTRIBUTOR,
                StudyRoles.SponsorRep => AzureRoleIds.READ,
                StudyRoles.VendorAdmin => AzureRoleIds.READ,
                StudyRoles.VendorContributor => AzureRoleIds.READ,
                _ => null,
            };
        }
    }
}
