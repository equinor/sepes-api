using Sepes.Infrastructure.Constants;
using Sepes.Infrastructure.Constants.CloudResource;
using Sepes.Infrastructure.Model;
using Sepes.Infrastructure.Util;
using Sepes.RestApi.IntegrationTests.TestHelpers;
using Sepes.Tests.Common.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sepes.RestApi.IntegrationTests.Setup.Seeding
{
    public static class StudySeed
    {
        public static async Task<Study> CreatedByCurrentUser(
            string name = StudyConstants.CREATED_BY_ME_NAME,
            string vendor = StudyConstants.CREATED_BY_ME_VENDOR,
            string wbs = StudyConstants.CREATED_BY_ME_WBS,
            bool restricted = false,
            int userId = TestUserConstants.COMMON_CUR_USER_DB_ID,
            string currentUserRole = null,
            bool addDatasets = false
            )
        {
            var study = StudyBasic(name, vendor, wbs, restricted);
            study.ResultsAndLearnings = "Results and learnings";
            AddParticipant(study, userId, StudyRoles.StudyOwner);

            if (!String.IsNullOrWhiteSpace(currentUserRole))
            {
                AddParticipant(study, userId, currentUserRole);
            }
            AddDatasetsIfWanted(addDatasets, study);
            await SliceFixture.InsertAsync(study);          

            return study;
        }

        public static async Task<Study> CreatedByOtherUser(
            string name = StudyConstants.CREATED_BY_OTHER_NAME,
            string vendor = StudyConstants.CREATED_BY_OTHER_VENDOR,
            string wbs = StudyConstants.CREATED_BY_OTHER_WBS,
            bool restricted = false,
            int ownerUserId = TestUserConstants.COMMON_ALTERNATIVE_STUDY_OWNER_DB_ID,
            int userId = TestUserConstants.COMMON_CUR_USER_DB_ID,
            string currentUserRole = null,
            bool addDatasets = false)
        {
            var study = StudyBasic(name, vendor, wbs, restricted);
            study.ResultsAndLearnings = "Results and learnings";
            AddParticipant(study, ownerUserId, StudyRoles.StudyOwner);

            if (!String.IsNullOrWhiteSpace(currentUserRole))
            {
                AddParticipant(study, userId, currentUserRole);
            }

            AddDatasetsIfWanted(addDatasets, study);
            await SliceFixture.InsertAsync(study);          

            return study;
        }

        //public static void AddDatasetsIfWanted(bool addDatasets, Study study)
        //{
        //    if (addDatasets)
        //    {
        //        for (var counter = 0; counter <= 2; counter++)
        //        {                  
        //            var datasetName = $"ds-{counter}";
        //            var datasetClassification = (DatasetClassification)counter;

        //            study.StudyDatasets.Add(DatasetFactory.CreateStudySpecificRelation(study, datasetName, TestConstants.REGION, datasetClassification.ToString()));
        //        }
        //    }
        //}

        public static void AddDatasetsIfWanted(bool addDatasets, Study study)
        {
            if (addDatasets)
            {
                for (var counter = 0; counter <= 2; counter++)
                {
                    var datasetName = $"ds-{counter}";
                    var datasetClassification = (DatasetClassification)counter;

                    var datasetRelation = DatasetFactory.CreateStudySpecificRelation(study, datasetName, TestConstants.REGION, datasetClassification.ToString());                  
                    study.StudyDatasets.Add(datasetRelation);               
                }
            }
        }

        static Study StudyBasic(string name, string vendor, string wbs, bool restricted)
        {
            return new Study()
            {
                Name = name,
                Vendor = vendor,
                WbsCode = wbs,
                Restricted = restricted,
                CreatedBy = "seed",
                Created = DateTime.UtcNow,
                UpdatedBy = "seed",
                Updated = DateTime.UtcNow,
                Sandboxes = new List<Sandbox>(),
                StudyDatasets = new List<StudyDataset>(),
                Resources = new List<CloudResource>() { StudySpecificDatasetResourceGroup(name) },
                
            };
        }

        static CloudResource StudySpecificDatasetResourceGroup(string studyName)
        {
            var resourceGroupName = AzureResourceNameUtil.StudySpecificDatasetResourceGroup(studyName);

            return CloudResourceFactory.CreateResourceGroup(TestConstants.REGION, resourceGroupName, purpose: CloudResourcePurpose.StudySpecificDatasetContainer);

        }

        public static void AddParticipant(Study study, int userId, string role)
        {
            if (study.StudyParticipants == null)
            {
                study.StudyParticipants = new List<StudyParticipant>();
            }

            var newParticipant = new StudyParticipant()
            {
                UserId = userId,
                RoleName = role,
                CreatedBy = "seed",
                Created = DateTime.UtcNow
            };

            study.StudyParticipants.Add(newParticipant);
        }
    }
}
